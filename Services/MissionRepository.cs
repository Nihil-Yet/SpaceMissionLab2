using Microsoft.EntityFrameworkCore;
using SpaceMission.Data;
using SpaceMission.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceMission.Services
{
    public interface IMissionRepository
    {
        Task<List<Mission>> GetAllAsync();
        Task<Mission?> GetByIdAsync(int id);
        Task AddAsync(Mission mission);
        Task UpdateAsync(Mission mission);
        Task DeleteAsync(int id);
    }

    public class MissionRepository : IMissionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IPlanetaryData _earthData;

        public MissionRepository(ApplicationDbContext context, IPlanetaryData earthData)
        {
            _context = context;
            _earthData = earthData;
        }

        public async Task<List<Mission>> GetAllAsync()
        {
            var entities = await _context.Missions.ToListAsync();
            return entities.Select(ToMission).Where(m => m != null).Cast<Mission>().ToList();
        }

        public async Task<Mission?> GetByIdAsync(int id)
        {
            var entity = await _context.Missions.FindAsync(id);
            return entity == null ? null : ToMission(entity);
        }

        public async Task AddAsync(Mission mission)
        {
            var entity = ToEntity(mission);
            await _context.Missions.AddAsync(entity);
            await _context.SaveChangesAsync();
            mission.Id = entity.Id;
        }

        public async Task UpdateAsync(Mission mission)
        {
            var entity = await _context.Missions.FindAsync(mission.Id);
            if (entity != null)
            {
                UpdateEntity(entity, mission);
                _context.Missions.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Missions.FindAsync(id);
            if (entity != null)
            {
                _context.Missions.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        private Mission? ToMission(MissionEntity e)
        {
            if (e.MissionType == (int)MissionT.Orbital)
            {
                var mission = new OrbitalMission(
                    e.Name, e.Budget, e.Duration,
                    e.CurrHeight ?? 500,
                    e.TargetHeight ?? 500,
                    e.Inclination ?? 0,
                    (EnergySource)(e.EnergySource ?? 0),
                    _earthData
                );
                mission.Id = e.Id;
                return mission;
            }
            else if (e.MissionType == (int)MissionT.Planetary)
            {
                var mission = new PlanetaryMission(
                    e.Name, e.Budget, e.Duration,
                    e.Planet ?? "Unknown",
                    e.AtmoDensity ?? 100,
                    new LandingPoint(e.LandingPointName ?? "Default", e.LandingPointX ?? 0, e.LandingPointY ?? 0, e.LandingPointR ?? 0)
                );
                mission.Id = e.Id;
                return mission;
            }
            return null;
        }

        private MissionEntity ToEntity(Mission m)
        {
            var e = new MissionEntity
            {
                Id = m.Id,
                Name = m.Name,
                Budget = m.Budget,
                Duration = m.Duration,
                MissionType = (int)m.MissionType
            };

            if (m is OrbitalMission o)
            {
                e.CurrHeight = o.CurrHeight;
                e.TargetHeight = o.TargetHeight;
                e.Inclination = o.Inclination;
                e.EnergySource = (int)o.EnergySource;
            }
            else if (m is PlanetaryMission p)
            {
                e.Planet = p.Planet;
                e.AtmoDensity = p.AtmoDensity;
                e.LandingPointName = p.LandingPoint.Name;
                e.LandingPointX = p.LandingPoint.X;
                e.LandingPointY = p.LandingPoint.Y;
                e.LandingPointR = p.LandingPoint.R;
            }
            return e;
        }

        private void UpdateEntity(MissionEntity entity, Mission m)
        {
            entity.Name = m.Name;
            entity.Budget = m.Budget;
            entity.Duration = m.Duration;

            if (m is OrbitalMission o)
            {
                entity.CurrHeight = o.CurrHeight;
                entity.TargetHeight = o.TargetHeight;
                entity.Inclination = o.Inclination;
                entity.EnergySource = (int)o.EnergySource;
            }
            else if (m is PlanetaryMission p)
            {
                entity.Planet = p.Planet;
                entity.AtmoDensity = p.AtmoDensity;
                entity.LandingPointName = p.LandingPoint.Name;
                entity.LandingPointX = p.LandingPoint.X;
                entity.LandingPointY = p.LandingPoint.Y;
                entity.LandingPointR = p.LandingPoint.R;
            }
        }
    }
}
