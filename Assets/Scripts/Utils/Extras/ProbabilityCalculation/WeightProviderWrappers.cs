using System;
using System.Collections.Generic;


namespace Honeylab.Utils.ProbabilityCalculation
{
    public class FloatWeightProvider : IWeightProvider
    {
        private readonly IReadOnlyList<float> _list;


        public FloatWeightProvider(IReadOnlyList<float> list)
        {
            _list = list;
        }


        public int Count() => _list.Count;
        public float GetWeightAtIdx(int idx) => _list[idx];
    }


    public class IntWeightProvider : IWeightProvider
    {
        private readonly IReadOnlyList<int> _list;


        public IntWeightProvider(IReadOnlyList<int> list)
        {
            _list = list;
        }


        public int Count() => _list.Count;
        public float GetWeightAtIdx(int idx) => _list[idx];
    }


    public class WeightListWeightProvider<T> : IWeightProvider where T : IWeight
    {
        private readonly IReadOnlyList<T> _list;


        public WeightListWeightProvider(IReadOnlyList<T> list)
        {
            _list = list;
        }


        public int Count() => _list.Count;
        public float GetWeightAtIdx(int idx) => _list[idx].GetWeight();
    }


    public class FuncWeightProvider<T> : IWeightProvider
    {
        private readonly IReadOnlyList<T> _list;
        private readonly Func<T, float> _weightsFunc;


        public FuncWeightProvider(IReadOnlyList<T> list, Func<T, float> weightsFunc)
        {
            _list = list;
            _weightsFunc = weightsFunc;
        }


        public int Count() => _list.Count;


        public float GetWeightAtIdx(int idx) => _weightsFunc(_list[idx]);
    }
}
