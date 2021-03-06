﻿using System;
using System.Collections.Generic;
using System.Text;
using MusicRecognition;
using MusicRecognition.GaussianMixtureModels;
using System.IO;

namespace TestingMethods
{
    class Program
    {
        // GMM
        private static int numberOfDistributions = 4;
        private static double treshold = 0.001;
        // knn
        private static int k = 6;
        // Random Decision Forest
        private static int classesNum = 4;
        private static double rCoefficient = 0.5;
        private static int treesNum = 50;
        // testing
        private static int testsCount = 100;
        private static string filePath = @"D:\GMM_multi_D" + numberOfDistributions + ".txt";     // path of file where results will be saved
        private static int crossValidationSets = 4;
		private static AbstractClassification classificator = new GMM(numberOfDistributions, treshold);
		//private static AbstractClassification classificator = new kNN(k);
		//private static AbstractClassification classificator = new RandomDecisionForest(classesNum, treesNum, rCoefficient);

        public static int Main()
        {
            for (int i=0; i<testsCount; i++)
            {
                Console.Clear();
                WriteCuteNotes();
                Console.WriteLine();
                Console.WriteLine("################## " + classificator.GetName());
                Console.WriteLine($"################## ({i}/{testsCount})");
                var validator = TestClassificator(classificator);
                ExportToFile(validator, filePath);
            }
            Console.Clear();
            Console.WriteLine("Exported!");
            Console.ReadLine();
            return 0;
        }

        private static ConfusionMatrixValidation TestClassificator(AbstractClassification classificator, bool printToStdout = false)
        {
            List<DataElement> dataSet = new List<DataElement>();
            using (Database db = new Database())
            {
                dataSet = db.GetElements();
            }

            classificator.Learn(dataSet);
            ConfusionMatrixValidation validator = new ConfusionMatrixValidation(classificator, dataSet, crossValidationSets);

            if (printToStdout)
            {
                Console.WriteLine($"Accuracy rate: {validator.AccuracyRate}");
                Console.WriteLine($"Correct: {validator.CorrectCount}");
                Console.WriteLine($"Errors: {validator.ErrorCount}");

                for (int i = 0; i < Enum.GetNames(typeof(SongGenre)).Length; i++)
                {
                    SongGenre genre = (SongGenre)i;
                    Console.WriteLine("------------");
                    Console.WriteLine(genre.ToString().ToUpper());
                    Console.WriteLine("------------");
                    Console.WriteLine($"ROC > True Positive rate: {validator.TPRate(genre)}");
                    Console.WriteLine($"ROC > False Positive rate: {validator.FPRate(genre)}");
                    Console.WriteLine($"True positive: {validator.TP(genre)}");
                    Console.WriteLine($"True negative: {validator.TN(genre)}");
                    Console.WriteLine($"False positive: {validator.FP(genre)}");
                    Console.WriteLine($"False negative: {validator.FN(genre)}");
                }
            }
            return validator;
        }

        private static void ExportToFile(ConfusionMatrixValidation validator, string filePath, string delimiter = "\t")
        {
            if (!File.Exists(filePath))
            {
                StringBuilder header = new StringBuilder($"AccuracyRate{delimiter}Correct{delimiter}Errors{delimiter}ComputationTime{delimiter}");
                for (int i = 0; i < Enum.GetNames(typeof(SongGenre)).Length; i++)
                {
                    SongGenre genre = (SongGenre)i;
                    header.Append($"Precision{genre}{delimiter}Recall{genre}{delimiter}TPRate{genre}{delimiter}FPRate{genre}{delimiter}TP{genre}{delimiter}TN{genre}{delimiter}FP{genre}{delimiter}FN{genre}{delimiter}");
                }

                File.AppendAllText(filePath ,header.ToString() + "\n");
            }
            string data = ConvertToString(validator, delimiter);
            File.AppendAllText(filePath, data + "\n");
        }
        private static string ConvertToString(ConfusionMatrixValidation validator, string delimiter)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(validator.AccuracyRate);
            sb.Append(delimiter);
            sb.Append(validator.CorrectCount);
            sb.Append(delimiter);
            sb.Append(validator.ErrorCount);
            sb.Append(delimiter);
            sb.Append(validator.ComputationTime);
            sb.Append(delimiter);

            for (int i=0; i< Enum.GetNames(typeof(SongGenre)).Length; i++)
            {
                SongGenre genre = (SongGenre)i;
                sb.Append(validator.Precision(genre));
                sb.Append(delimiter);
                sb.Append(validator.Recall(genre));
                sb.Append(delimiter);
                sb.Append(validator.TPRate(genre));
                sb.Append(delimiter);
                sb.Append(validator.FPRate(genre));
                sb.Append(delimiter);
                sb.Append(validator.TP(genre));
                sb.Append(delimiter);
                sb.Append(validator.TN(genre));
                sb.Append(delimiter);
                sb.Append(validator.FP(genre));
                sb.Append(delimiter);
                sb.Append(validator.FN(genre));
                sb.Append(delimiter);
            }

            return sb.ToString();
        }

        private static void WriteCuteNotes()
        {
            Console.WriteLine(@"────███");
            Console.WriteLine(@"────█─██");
            Console.WriteLine(@"────█──██ ");
            Console.WriteLine(@"────█───█───────────────────♫─────██");
            Console.WriteLine(@"────█───█────────────♫────────────█─█──█");
            Console.WriteLine(@"────█───██─────♫──────────────────█──██");
            Console.WriteLine(@"────██───█────────────────────────█");
            Console.WriteLine(@"────██──██────────████████─────████");
            Console.WriteLine(@"─────█──█─────────█──────█────██░▒██");
            Console.WriteLine(@"─────█─██─────────█──────██────████");
            Console.WriteLine(@"─────███──────────██──────█");
            Console.WriteLine(@"───██─█────────────█───████");
            Console.WriteLine(@"──██──████──────████──██░▒██");
            Console.WriteLine(@"─██──███─███───██░▒██──████__");
            Console.WriteLine(@"██──██─█──▒██───████");
            Console.WriteLine(@"█────█─██─░▒█");
            Console.WriteLine(@"██─────██─░▒█");
            Console.WriteLine(@"─██─────█─▒██ ");
            Console.WriteLine(@"──█████─████");
            Console.WriteLine(@"──────████");
        }
    }
}
