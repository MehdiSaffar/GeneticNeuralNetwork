using System;

    public class NeuralNetworkHelper{
        /// <summary>
        /// Applies the sigmoid function on the <paramref name="value"/>
        /// </summary>
        /// <param name="value">Value</param>
        public static double Sigmoid(double value) => 1.0 / (1.0 + Math.Exp(-value));

    }
