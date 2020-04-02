namespace MarxMLL2
{
    public interface IWeightsGenerator
    {
        double[] CreateRandomWeights(int numElements);
        double[] CreateRandomWeightsPRNGSHA512(int numElements);
    }
}