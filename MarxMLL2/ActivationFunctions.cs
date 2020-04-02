using System;
using System.Collections.Generic;
using System.Text;

namespace MarxMLL2
{
    public class ActivationFunctions
    {
        public double Identity(double x)
        {
            return x;
        }

        public double BinaryStep(double x)
        {
            return x < 0 ? 0 : 1;
        }

        public double Logistic(double x)
        {
            return 1 / (1 + Math.Pow(Math.E, -x));
        }

        public double DLogistic(double x)
        {
            return Logistic(x) * (1 - Logistic(x));
        }

        public double Tanh(double x)
        {
            return 2 / (1 + Math.Pow(Math.E, -(2 * x))) - 1;
        }

        public double DTanh(double x)
        {
            return 1 - Math.Pow(Tanh(x), 2);
        }
        /*
        public double ArcTan(double x)
        {
            return Math.Atan(x);
        }
        */
        public double DArcTan(double x)
        {
            return 1 / Math.Pow(x, 2) + 1;
        }

        public static double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        public static double SigmoidDerivative(double x)
        {
            return x * (1 - x);
        }

        public double PReLU(double x, double a)
        {
            return x < 0 ? a * x : x;
        }

        public double DPReLU(double x, double a)
        {
            return x < 0 ? a : 1;
        }

        public double ELU(double x, double a)
        {
            return x < 0 ? a * (Math.Pow(Math.E, x) - 1) : x;
        }

        public double DELU(double x, double a)
        {
            return x < 0 ? ELU(x, a) + a : 1;
        }

        public double SoftPlus(double x)
        {
            return Math.Log(Math.Exp(x) + 1);
        }

        public double DSoftPlus(double x)
        {
            return Logistic(x);
        }

        public double BentIdentity(double x)
        {
            return (((Math.Sqrt(Math.Pow(x, 2) + 1)) - 1) / 2) + x;
        }

        public double DBentIdentity(double x)
        {
            return (x / (2 * Math.Sqrt(Math.Pow(x, 2) + 1))) + 1;
        }

        public double SoftExponential(double x, double alpha = 0.0)
        {

            // """Soft Exponential activation function by Godfrey and Gashler
            // See: https://arxiv.org/pdf/1602.01321.pdf
            // α == 0:  f(α, x) = x
            // α  > 0:  f(α, x) = (exp(αx)-1) / α + α
            // α< 0:  f(α, x) = -ln(1 - α(x + α)) / α
            // """

            if (alpha == 0)
                return x;
            else if (alpha > 0)
                return alpha + (Math.Exp(alpha * x) - 1.0) / alpha;
            else
                return -Math.Log(1 - alpha * (x + alpha)) / alpha;
        }

        public double Sinusoid(double x)
        {
            return Math.Sin(x);
        }

        public double DSinusoid(double x)
        {
            return Math.Cos(x);
        }

        public double Sinc(double x)
        {
            return x == 0 ? 1 : Math.Sin(x) / x;
        }

        public double DSinc(double x)
        {
            return x == 0 ? 0 : (Math.Cos(x) / x) - (Math.Sin(x) / Math.Pow(x, 2));
        }

        public double Gaussian(double x)
        {
            return Math.Pow(Math.E, Math.Pow(-x, 2));
        }

        public double DGaussian(double x)
        {
            return -2 * x * Math.Pow(Math.E, Math.Pow(-x, 2));
        }

        public double Bipolar(double x)
        {
            return x < 0 ? -1 : 1;
        }

        public double BipolarSigmoid(double x)
        {
            return (1 - Math.Exp(-x)) / (1 + Math.Exp(-x));
        }

        public double DBipolarSigmoid(double x)
        {
            return 0.5 * (1 + BipolarSigmoid(x)) * (1 - BipolarSigmoid(x));
        }

        public double Scaler(double x, double min, double max)
        {
            return (x - min) / (max - min);
        }

        public double LogSigmoid(double x)
        {
            if (x < -45.0) return 0.0;
            else if (x > 45.0) return 1.0;
            else return 1.0 / (1.0 + Math.Exp(-x));
        }

        public double HyperbolicTangtent(double x)
        {
            if (x < -45.0) return -1.0;
            else if (x > 45.0) return 1.0;
            else return Math.Tanh(x);
        }

        public double LogisticFunctionSteep(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-4.9 * x));
        }

        public double LogisticApproximantSteep(double x)
        {
            return 1.0 / (1.0 + Exp(-4.9 * x));
        }

        public double SoftSign(double x)
        {
            return 0.5 + (x / (2.0 * (0.2 + Math.Abs(x))));
        }

        public double PolynomialApproximant(double x)
        {
            x *= 4.9;
            double x2 = x * x;
            double e = 1.0 + Math.Abs(x) + x2 * 0.555 + x2 * x2 * 0.143;

            double f = (x > 0) ? (1.0 / e) : e;
            return 1.0 / (1.0 + f);
        }

        public double QuadraticSigmoid(double x)
        {
            const double t = 0.999;
            const double a = 0.00001;

            double sign = Math.Sign(x);
            x = Math.Abs(x);

            double y = 0;
            if (x >= 0 && x < t)
            {
                y = t - ((x - t) * (x - t));
            }
            else //if (x >= t) 
            {
                y = t + (x - t) * a;
            }

            return (y * sign * 0.5) + 0.5;
        }

        public double ReLU(double x)
        {
            double y;
            if (x > 0.0)
            {
                y = x;
            }
            else
            {
                y = 0.0;
            }
            return y;
        }

        public double LeakyReLU(double x)
        {
            const double a = 0.001;

            double y;
            if (x > 0.0)
            {
                y = x;
            }
            else
            {
                y = x * a;
            }
            return y;
        }

        public double LeakyReLUShifted(double x)
        {
            const double a = 0.001;
            const double offset = 0.5;

            double y;
            if (x + offset > 0.0)
            {
                y = x + offset;
            }
            else
            {
                y = (x + offset) * a;
            }
            return y;
        }

        public double SReLU(double x)
        {
            const double tl = 0.001; // threshold (left).
            const double tr = 0.999; // threshold (right).
            const double a = 0.00001;

            double y;
            if (x > tl && x < tr)
            {
                y = x;
            }
            else if (x <= tl)
            {
                y = tl + (x - tl) * a;
            }
            else
            {
                y = tr + (x - tr) * a;
            }

            return y;
        }

        public double SReLUShifted(double x)
        {
            const double tl = 0.001; // threshold (left).
            const double tr = 0.999; // threshold (right).
            const double a = 0.00001;
            const double offset = 0.5;

            double y;
            if (x + offset > tl && x + offset < tr)
            {
                y = x + offset;
            }
            else if (x + offset <= tl)
            {
                y = tl + ((x + offset) - tl) * a;
            }
            else
            {
                y = tr + ((x + offset) - tr) * a;
            }

            return y;
        }

        public double ArcTan(double x)
        {
            const double halfpi = Math.PI / 2.0;
            const double piinv = 1.0 / Math.PI;
            return (Math.Atan(x) + halfpi) * piinv;

        }

        public double TanH(double x)
        {
            return (Math.Tanh(x) + 1.0) * 0.5;
        }

        public double ArcSinH(double x)
        {
            return 1.2567348023993685 * ((Asinh(x) + 1.0) * 0.5);
        }
        private double Exp(double val)
        {
            long tmp = (long)(1512775 * val + (1072693248 - 60801));
            return BitConverter.Int64BitsToDouble(tmp << 32);
        }

        public double ScaledELU(double x)
        {
            double alpha = 1.6732632423543772848170429916717;
            double scale = 1.0507009873554804934193349852946;

            double y;
            if (x >= 0)
            {
                y = scale * x;
            }
            else
            {
                y = scale * ((alpha * Math.Exp(x)) - alpha);
            }

            return y;
        }

        public double MaxMinusOne(double x)
        {
            double y;
            if (x > -1)
            {
                y = x;
            }
            else
            {
                y = -1;
            }
            return y;
        }

        private double Asinh(double value)
        {
            return Math.Log(value + Math.Sqrt((value * value) + 1), Math.E);
        }
    }
}
