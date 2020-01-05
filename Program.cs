using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace SpellingBee
{
    class Program
    {
        // Private class to hold a word in the wordList.  Contains the Text of the word, along with
        // a uint bit flag (CharFLags) which indicates which letters of the alphabet are used in the word
        private class Word
        {
            public string Text { get; set; }
            public uint CharFlags { get; set; }
        }

        // An array for mapping an integer to a letter of the alphabet.  Used for printing results.
        private static readonly char[] chars = new char[]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

        // Holds the relevant words from the given word list, along with a bit flag for each indicating which letters
        // of the alphabet are used in the word.
        private static List<Word> wordList = new List<Word>();

        // This function builds the internal wordList from the given file.  Words of length less than four, words
        // containing an "s", and words that use more than 7 letters are omitted.  A bit flag indicating which letters
        // are used is also computed for each word in the internal list.
        private static void BuildWordList()
        {
            using (StreamReader reader = new StreamReader(@"..\..\..\RawWordList.txt"))
            {
                string buffer;
                while ((buffer = reader.ReadLine()) != null)
                {
                    if ((buffer.Length > 3) && (!buffer.Contains('s')))
                    {
                        int numCharsUsed = 0;
                        uint flags = 0;
                        foreach (char c in buffer)
                        {
                            uint flag = (uint)1 << (c - 'a');
                            if ((flags & flag) == 0)
                            {
                                flags |= flag;
                                numCharsUsed++;
                            }
                        }
                        if (numCharsUsed <= 7)
                            wordList.Add(new Word { Text = buffer, CharFlags = flags });
                    }
                }
            }
        }

        // FOr a given set of 7 letters, with one of them designated as the center letter, find all the words
        // in the wordList that can be built from this set and compute the total score.
        private static int ComputeScore(int[] ints, int centerCharIndex, List<Word> validWordList)
        {
            bool hadPangram = false;
            uint mask7 = 0;
            for (int i = 0; i < ints.Length; i++)
                mask7 |= (uint)1 << ints[i];
            uint maskC = (uint)1 << ints[centerCharIndex];
            int score = 0;
            foreach (Word word in wordList)
            {
                if (((maskC & word.CharFlags) != 0) && ((mask7 | word.CharFlags) == mask7))
                {
                    validWordList.Add(word);
                    if (word.Text.Length == 4)
                        score++;
                    else
                        score += word.Text.Length;
                    if (mask7 == word.CharFlags)
                    {
                        score += 7;
                        hadPangram = true;
                    }
                }
            }
            if (!hadPangram)
                score = -1;
            return score;
        }

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            BuildWordList();

            // For all combinations of 7 letters, and for using each letter in each comination as the center letter,
            // compute the maximum score.
            int maxScore = 0;  // The current maximum score
            int[] maxInts = null;  // The 7-letter comination for the current maxScore
            int maxCenterCharIdx = -1;  // The index of the center letter for the current maxScore
            List<Word> maxWordList = new List<Word>();  // The word list for current maxScore

            for (int i=0; i<20; i++)
                for (int j=i+1; j<21; j++)
                    for(int k=j+1; k<22; k++)
                        for (int l=k+1; l<23; l++)
                            for (int m=l+1; m<24; m++)
                                for (int n=m+1; n<25; n++)
                                    for (int o=n+1; o<26; o++)
                                    {
                                        int[] ints = new int[] { i, j, k, l, m, n, o };
                                        for (int idx = 0; idx < ints.Length; idx++)
                                        {
                                            List<Word> validWordList = new List<Word>();
                                            int score = ComputeScore(ints, idx, validWordList);
                                            if (score > maxScore)
                                            {
                                                maxScore = score;
                                                maxInts = ints;
                                                maxCenterCharIdx = idx;
                                                maxWordList = validWordList;
                                                Console.WriteLine("New max score: " + score + "; for letters: " + chars[i] + "," + chars[j] + "," + chars[k] + "," + chars[l] + "," + chars[m] + "," + chars[n] + "," + chars[o] + "; center: " + chars[ints[idx]]);
                                            }
                                        }
                                    }
            Console.WriteLine();
            Console.Write("Max score: " + maxScore + " for letters: ");
            for (int i = 0; i < maxInts.Length; i++)
            {
                Console.Write(chars[maxInts[i]]);
                if (i < maxInts.Length - 1)
                    Console.Write(", ");
            }
            Console.WriteLine("; center: " + chars[maxInts[maxCenterCharIdx]]);
            Console.WriteLine("Word list:");
            foreach (Word word in maxWordList)
                Console.WriteLine("    " + word.Text);
            watch.Stop();
            Console.WriteLine();
            Console.WriteLine("Elapsed time: " + String.Format("{0:0.00}", ((double)watch.ElapsedMilliseconds / 60000)) + " minutes");
        }
    }
}
