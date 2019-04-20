using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace AggregateVsTolookup
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TestRunner>();
        }
    }

    [ClrJob, MonoJob, CoreJob] // 可以針對不同的 CLR 進行測試
    [MinColumn, MaxColumn]
    [MemoryDiagnoser]
    public class TestRunner
    {
        private readonly TestClass _test = new TestClass();

        public TestRunner()
        {
        }

        [Benchmark]
        public void TestMethod1() => _test.TestMethod1();

        [Benchmark]
        public void TestMethod2() => _test.TestMethod2();
    }

    public class TestClass
    {
        Random r = new Random((int) DateTime.Now.Ticks);

        IList<Person> data = Enumerable.Repeat(new Person(), 100)
                                       .Select((a, idx) => new Person() { Age = idx })
                                       .ToList();

        public void TestMethod1()
        {
            int tempSum = 0, tempGroup = 0;
            var result = data.ToLookup((a) =>
            {
                if (tempSum > 100)
                {
                    tempSum = a.Age;
                    return ++tempGroup;
                }
                else
                {
                    tempSum += a.Age;
                    return tempGroup;
                }
            });

            foreach (var root in result)
            {
                //Console.WriteLine("--------------");
                foreach (var item in root)
                {
                    //Console.WriteLine(item.Age);
                    var a = item.Age;
                }
            }
        }

        public void TestMethod2()
        {
            var result = data.Aggregate(
                                        seed: new DTO { GroupPersons = new Dictionary<int, List<Person>>() },
                                        func: (dto, item) => {

                                                  if (dto.tempSum > 100)
                                                  {
                                                      dto.tempGroup++;
                                                      dto.tempSum = 0;
                                                  }

                                                  if (dto.GroupPersons.ContainsKey(dto.tempGroup) == false)
                                                  {
                                                      dto.GroupPersons.Add(dto.tempGroup, new List<Person> { });
                                                  }

                                                  dto.tempSum += item.Age;
                                                  dto.GroupPersons[dto.tempGroup].Add(item);

                                                  return dto;
                                              },
                                        resultSelector: dto => dto.GroupPersons
                                       );

            foreach (var root in result.Values)
            {
                //Console.WriteLine("--------------");
                foreach ( var item in root )
                {
                    //Console.WriteLine(item.Age);
                    var a = item.Age;
                }
            }
        }
    }

    public class DTO
    {
        public int                           tempSum      { get; set; }
        public int                           tempGroup    { get; set; }
        public Dictionary<int, List<Person>> GroupPersons { get; set; }
    }

    public class Person
    {
        public static Person Empty => new Person();
        public        string Name  { get; set; }
        public        int    Age   { get; set; }
        public        string City  { get; set; }
    }
}