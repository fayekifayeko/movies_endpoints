using AutoMapper;
using NetTopologySuite.Geometries;

namespace MoviesApi
{
    internal class AutoMapperProfiles : Profile
    {
        private GeometryFactory geometryFactory;

        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            this.geometryFactory = geometryFactory;
        }
    }
}