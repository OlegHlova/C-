using Xunit;

namespace TestLagguerre.Tests
{


    public class LaggerFrameworkTests
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

        [Theory]
        [InlineData(0, 5, 100000, 71.6666)]
        [InlineData(-5, 5, 100000, 93.3333)]
        [InlineData(2, 10, 100000, 434.6666)]
        [InlineData(-1, 1, 1000000, 2.6666)]
        public void Integral_Tests(double a, double b, int n, double expected)
        {
            // Arrange
            var laggerFramework = new LaggerFramework();
            double Func(double x) => Math.Pow(x,2) + 2*x + 1;

            // Act
            var result = laggerFramework.Quadratic(Func, a, b, n);

            // Assert
            Assert.InRange(result, expected-0.1, expected+0.1); 
        }

        [Theory]
        [InlineData(1, 0, 0.7358)]
        [InlineData(3, 7, -3.553)]
        [InlineData(1, 2, 0.7357)]
        [InlineData(3, 0, 0.1)]       
        public void Lagger_Tests(double t, int n, double expected)
        {

            var lager = new LaggerFramework();

            double result = lager.Lagger(t, n);

            Assert.Equal(expected, Math.Round(result, 4), 3);
        }

        [Theory]
        [InlineData(1, 2, 0.1,20)]
        [InlineData(1, 3, 0.1, 30)]
        public void Tabulation_Lager_Length_Tests(int n, double t, double s, double expected)
        {
            var lager = new LaggerFramework();

            double result = lager.LagerTabulation(n, t, s).Count();

            Assert.Equal(expected, result);

        }

        [Theory]
        [InlineData(1, 2, 0.1, 1.0858)]
        [InlineData(5,2,0.1,-0.545)]
        public void Tabulation_Lager_Tests(int n, double t, double s, double expected)
        {
            var lager = new LaggerFramework();

            double result = lager.LagerTabulation(n, t, s)[0.1];

            Assert.Equal(expected, result, 3);

        }

        [Theory]
        [InlineData(20, 0.001, 0.093, 21)]
        public void Experiment_Tests(int n, double e, double expected_number, int expected_count)
        {
            var lager = new LaggerFramework();

            var result = lager.Experiment(n,e);

            var res_num = result.Item1;

            var res_count = result.Item2.Count();
            

            Assert.Equal(expected_number, res_num, 3);
            Assert.Equal(expected_count, res_count);
        }

        [Theory]
        [InlineData(1, 0.00016)]
        public void Lager_Transformation_Tests(int n, double expected)
        {
            var lager = new LaggerFramework();

            var result = lager.LaggerTransformation(F, n);

            Assert.Equal(result, expected, 5);
        }

        [Theory]
        [InlineData(0.1, 0.000616)]
        public void Lager_Reverse_Transformation_Tests(double t, double expected)
        {
            var lager = new LaggerFramework();
            List<double> tabulation = lager.TabulateTransformation(F, 5);
            
            double result = lager.ReversedLaggerTransformation(tabulation, t);
            Assert.Equal(expected, result, 5);
        }

        [Theory]
        [InlineData(1, 1, 1, 0.39894)]
        [InlineData(1, 2, 3, 0.12579)]
        public void Gauss_Tests(double t, double mu, double l, double expected)
        {
            var lager = new LaggerFramework();
            
            var result = lager.NormalDistribution(t, mu, l);

            Assert.Equal(expected, result, 5);
        }
    }
}
