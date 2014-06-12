using System;
using System.Reflection;
using MoreLinq;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WastelandA23.Marshalling
{
    public static class TypeMapper
    {

        private struct TypeMap
        {
            public Type Destination;
            public Type Source;
            public TypeMap(Type Destination, Type Source)
            {
                this.Destination = Destination;
                this.Source = Source;
            }
        }


        public static void Map<TSource,TDestination>
            (TSource Source, TDestination Destination)
        {
            var srcTypeList = Assembly.GetAssembly(typeof(TSource)).DefinedTypes;
            var destTypeList = Assembly.GetAssembly(typeof(TDestination)).DefinedTypes;

            var matchedTypes = srcTypeList.Join
                (
                    destTypeList,
                    _ => _.Name,
                    _ => _.Name,
                    (src, dest) => new 
                    { 
                        TSource = src,
                        TDerivedSrc = Marshaller.findAllDerivedTypes(src),
                        TDestination = dest,
                        TDerivedDst = Marshaller.findAllDerivedTypes(dest)
                    }
                );

            matchedTypes.ForEach(_ =>
            {
                Mapper.CreateMap(_.TSource, _.TDestination);
                Mapper.CreateMap(_.TDestination, _.TSource);
            });

            var matchDerivedTypes = matchedTypes.Where(
                _ => _.TDerivedSrc.Any() && _.TDerivedDst.Any());

            Dictionary<TypeMap, List<TypeMap>> matchedDerivedTypDict 
                = new Dictionary<TypeMap, List<TypeMap>>();

            matchDerivedTypes.ForEach(_ =>
            {
                matchedDerivedTypDict.Add
                    (
                        new TypeMap{Source=_.TSource, Destination=_.TDestination},
                        _.TDerivedSrc.Join
                        (
                            _.TDerivedDst,
                            __ => __.Name,
                            __ => __.Name,
                            (src, dst) => new TypeMap 
                            {
                                Source=src,
                                Destination=dst 
                            }).ToList());
            });

            matchedDerivedTypDict.ForEach(_ =>
            {
                _.Value.ForEach
                    (
                        __ => Mapper.GetAllTypeMaps()
                            .First
                            (
                                ___ => ___.SourceType == _.Key.Source)
                                .IncludeDerivedTypes
                                (
                                    __.Source, 
                                    __.Destination
                                ));
                _.Value.ForEach
                    (
                        __ => Mapper.GetAllTypeMaps()
                            .First
                            (
                                ___ => ___.SourceType == _.Key.Destination)
                                .IncludeDerivedTypes
                                (
                                    __.Destination,
                                    __.Source
                                ));

            });

        }

    }
}
