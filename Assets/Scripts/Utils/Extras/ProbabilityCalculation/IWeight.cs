namespace Honeylab.Utils.ProbabilityCalculation
{
    public interface IWeight
    {
        float GetWeight();
    }


    public interface IWeightProvider
    {
        int Count();
        float GetWeightAtIdx(int idx);
    }
}
