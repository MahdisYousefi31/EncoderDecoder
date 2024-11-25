using System;
using System.IO;

namespace EncoderDecoder
{
    public interface ICoder
    {
        string EncodeUsingFirstFormula(string message, string senderName, string receiverName);
        string DecodeUsingFirstFormula(string encodedMessage, string senderName, string receiverName);

        string EncodeUsingSecondFormula(string message, string senderName, string receiverName);
        string DecodeUsingSecondFormula(string encodedMessage, string senderName, string receiverName);
        void SaveToFile(string message, string fileName);
        string LoadFromFile(string fileName);
    }

    public class EncoderDecoder : ICoder
    {
        public string EncodeUsingFirstFormula(string message, string senderName, string receiverName)
        {
            int key = CalculateKeyUsingFirstFormula(senderName, receiverName);
            return EncodeMessage(message, key);
        }

        public string DecodeUsingFirstFormula(string encodedMessage, string senderName, string receiverName)
        {
            int key = CalculateKeyUsingFirstFormula(senderName, receiverName);
            return DecodeMessage(encodedMessage, key);
        }

        public string EncodeUsingSecondFormula(string message, string senderName, string receiverName)
        {
            int key = CalculateKeyUsingSecondFormula(senderName, receiverName);
            return EncodeMessage(message, key);
        }

        public string DecodeUsingSecondFormula(string encodedMessage, string senderName, string receiverName)
        {
            int key = CalculateKeyUsingSecondFormula(senderName, receiverName);
            return DecodeMessage(encodedMessage, key);
        }

        private int CalculateKeyUsingFirstFormula(string senderName, string receiverName)
        {
            int senderValue = GetNameSum(senderName);
            int receiverValue = GetNameSum(receiverName);
            return (senderValue + receiverValue) % 52;
        }

        private int CalculateKeyUsingSecondFormula(string senderName, string receiverName)
        {
            int senderValue = GetNameSum(senderName);
            int receiverValue = GetNameSum(receiverName);
            int product = senderValue * receiverValue;
            int sum = senderValue + receiverValue;
            return sum == 0 ? 0 : product / sum;
        }

        private int GetNameSum(string name)
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            int sum = 0;
            foreach (char letter in name)
            {
                sum += alphabet.IndexOf(letter) + 1;
            }
            return sum;
        }

        private string EncodeMessage(string message, int key)
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            string encoded = "";
            foreach (char letter in message)
            {
                if (alphabet.Contains(letter))
                {
                    int originalPos = alphabet.IndexOf(letter);
                    int newPos = (originalPos + key) % 52;
                    encoded += alphabet[newPos];
                }
                else
                {
                    encoded += letter;
                }
            }
            return encoded;
        }

        private string DecodeMessage(string encodedMessage, int key)
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            string decoded = "";
            foreach (char letter in encodedMessage)
            {
                if (alphabet.Contains(letter))
                {
                    int encodedPos = alphabet.IndexOf(letter);
                    int originalPos = (encodedPos - key + 52) % 52;
                    decoded += alphabet[originalPos];
                }
                else
                {
                    decoded += letter;
                }
            }
            return decoded;
        }

        public void SaveToFile(string message, string fileName)
        {
            File.WriteAllText(fileName, message);
            Console.WriteLine($"Message saved to {fileName}");
        }

        public string LoadFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName);
            }
            else
            {
                Console.WriteLine($"Error: {fileName} not found.");
                return string.Empty;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ICoder coder = new EncoderDecoder();
            Console.WriteLine("Who are you? Type 'Sender' or 'Receiver':");
            string role = Console.ReadLine()?.Trim().ToLower();

            if (role == "sender")
            {
                Console.Write("Enter your name (Sender): ");
                string senderName = Console.ReadLine();

                Console.Write("Enter the receiver's name: ");
                string receiverName = Console.ReadLine();

                Console.Write("Enter the message to encode: ");
                string message = Console.ReadLine();

                string encodedMessage1 = coder.EncodeUsingFirstFormula(message, senderName, receiverName);
                string encodedMessage2 = coder.EncodeUsingSecondFormula(message, senderName, receiverName);

                Console.WriteLine($"Encoded using First Formula: {encodedMessage1}");
                Console.WriteLine($"Encoded using Second Formula: {encodedMessage2}");

                coder.SaveToFile(encodedMessage1, "encodedMessage1.txt");
                coder.SaveToFile(encodedMessage2, "encodedMessage2.txt");
                Console.WriteLine("Messages have been saved as 'encodedMessage1.txt' and 'encodedMessage2.txt'.");
            }
            else if (role == "receiver")
            {
                Console.Write("Enter the sender's name: ");
                string senderName = Console.ReadLine();

                Console.Write("Enter your name (Receiver): ");
                string receiverName = Console.ReadLine();

                Console.Write("Enter the file name to decode (e.g., 'encodedMessage1.txt'): ");
                string fileName = Console.ReadLine();

                string encodedMessage = coder.LoadFromFile(fileName);
                if (string.IsNullOrEmpty(encodedMessage))
                {
                    Console.WriteLine("Error: File not found or empty.");
                    return;
                }

                string decodedMessage1 = coder.DecodeUsingFirstFormula(encodedMessage, senderName, receiverName);
                string decodedMessage2 = coder.DecodeUsingSecondFormula(encodedMessage, senderName, receiverName);

                Console.WriteLine($"Decoded using First Formula: {decodedMessage1}");
                Console.WriteLine($"Decoded using Second Formula: {decodedMessage2}");
            }
            else
            {
                Console.WriteLine("Invalid role. Please restart the program.");
            }
        }
    }
}
