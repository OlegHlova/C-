using System;
using System.Collections.Generic;
using System.Linq;

public class LaggerFramework
{
    public  double beta { get; set; }
    public  double sigma { get; set; }

    public LaggerFramework(double b = 2, double s = 4)
    {
        if (b < 0)
            throw new ArgumentException("beta must be positive");

        if (s < b)
            throw new ArgumentException("sigma must be greater than beta");

        beta = b;
        sigma = s;
    }

    public double Quadratic(Func<double, double> f, double a, double b, int N = 10000)
    {
        var x = Enumerable.Range(0, N + 1).Select(i => a + (b - a) * i / N);
        return x.Select(f).Sum() * Math.Abs(b - a) / N;
    }

    public double Lagger(double t, int n)
    {
        double l_0 = Math.Sqrt(sigma) * Math.Exp(-beta * t / 2);
        double l_1 = Math.Sqrt(sigma) * (1 - sigma * t) * Math.Exp(-beta * t / 2);

        if (n == 0)
            return l_0;
        if (n == 1)
            return l_1;

        for (int j = 2; j <= n; j++)
        {
            double temp = l_1;
            l_1 = (2 * j - 1 - t * sigma) / j * l_1 - (j - 1) * l_0 / j;
            l_0 = temp;
        }
        return l_1;
    }

    
    public Dictionary<double, double> LagerTabulation(int n, double t, double s = 0.1)
    {

        var result = new Dictionary<double, double>();
        for (double i = 0; i < t; i += s)
        {
            result.Add(i, Lagger(i, n));
        }

        return result;
    }

    public (double, List<double>) Experiment(int n = 20, double eps = 0.001)
    {
        double t = 0;
        List<double> res = new List<double>();

        while (true)
        {
            t += 0.001;

            for (int i = 0; i <= n; i++)
            {
                double x = Math.Abs(Lagger(t, n));
                if (x < eps)
                {
                    res.Add(x);
                    if (i == n)
                        return (t, res);
                }
                else
                {
                    break;
                }
            }
        }
    }
    public  double LaggerTransformation(Func<double, double> f, int n, double beta = 2, double gamma = 4)
    {
        Func<double, double> integrand = t => f(t) * Lagger(t, n) * Math.Exp(-t * (gamma - beta));
        double b = Experiment().Item1;
        return Quadratic(integrand, 0, b);
    }

    public List<double> TabulateTransformation(Func<double, double> f, int N)
    {
        List<double> results = new List<double>();
        for (int n = 0; n <= N; n++)
        {
            double transformation = LaggerTransformation(f, n);
            results.Add(transformation);
        }

        return results;
    }
    public  double ReversedLaggerTransformation(List<double> lst, double t)
    {
        return lst.Select((value, index) => value * Lagger(t, index)).Sum();
    }

   
    public  double NormalDistribution(double t, double mu, double lambda)
    {
        return (Math.Exp(-(t - mu) * (t - mu) / (2 * lambda * lambda))) / (lambda * Math.Sqrt(2 * Math.PI));
    }
}

namespace Program
{
    class Task
    {
        public static double F(double t)
        {
            if (t >= 0 && t <= 2 * Math.PI)
            {
                return Math.Sin(t - Math.PI / 2) + 1;
            }
            else
            {
                return 0;
            }
        }
        static void Main(string[] args)
        {
            double beta = 2;
            double sigma = 4;
            int n = 2;
            double t = 1;

            var lager = new LaggerFramework(beta, sigma);

            double laggerValue = lager.Lagger(t, n);
            Console.WriteLine($"Lagger value at t={t} with n={n}, beta={beta}, sigma={sigma}: {laggerValue}");

            var tab_res = lager.LagerTabulation(1, 0.2, 0.1);
            Console.WriteLine(tab_res[0.1]);
            double mu = 9;
            double lambda = 3;
            double tValue = 2;

            double laggerTransform = lager.LaggerTransformation(F, n, beta, sigma);
            Console.WriteLine($"Lagger transformation with n={n}, beta={beta}, sigma={sigma}: {laggerTransform}");

            double gaussianValue = lager.NormalDistribution(tValue, mu, lambda);
            Console.WriteLine($"Gaussian distribution value at t={tValue} with mu={mu}, lambda={lambda}: {gaussianValue}");

            double mu2 = 18;
            double gaussianValue2 = lager.NormalDistribution(tValue, mu2, lambda);
            Console.WriteLine($"Gaussian distribution value at t={tValue} with mu={mu2}, lambda={lambda}: {gaussianValue2}");



        }
    }

}

    

