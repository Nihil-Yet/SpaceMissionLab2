using Microsoft.EntityFrameworkCore;
using SpaceMission.Data;
using SpaceMission.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

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
            Console.WriteLine($"DB path: {System.IO.Path.GetFullPath("missions.db")}");
        }

        public async Task<List<Mission>> GetAllAsync()
        {
            var bases = await _context.Missions
                .AsNoTracking()
                .Include(m => m.Orbital)
                .Include(m => m.Planetary)
                .ToListAsync();

            return bases.Select(ToMission).Where(m => m != null).Cast<Mission>().ToList();
        }

        public async Task<Mission?> GetByIdAsync(int id)
        {
            var baseMission = await _context.Missions
                .Include(m => m.Orbital)
                .Include(m => m.Planetary)
                .FirstOrDefaultAsync(m => m.Id == id);
            return baseMission == null ? null : ToMission(baseMission);
        }

        public async Task AddAsync(Mission mission)
        {
            var baseEntity = new MissionBase
            {
                Name = mission.Name,
                Budget = mission.Budget,
                Duration = mission.Duration,
                MissionType = (int)mission.MissionType
            };

            if (mission is OrbitalMission o)
            {
                baseEntity.Orbital = new OrbitalMissionEntity
                {
                    CurrHeight = o.CurrHeight,
                    TargetHeight = o.TargetHeight,
                    Inclination = o.Inclination,
                    EnergySource = (int)o.EnergySource
                };
            }
            else if (mission is PlanetaryMission p)
            {
                baseEntity.Planetary = new PlanetaryMissionEntity
                {
                    Planet = p.Planet,
                    AtmoDensity = p.AtmoDensity,
                    LandingPointName = p.LandingPoint.Name,
                    LandingPointX = p.LandingPoint.X,
                    LandingPointY = p.LandingPoint.Y,
                    LandingPointR = p.LandingPoint.R
                };
            }

            _context.Missions.Add(baseEntity);
            await _context.SaveChangesAsync();
            mission.Id = baseEntity.Id;
        }

        public async Task UpdateAsync(Mission mission)
        {
            var baseEntity = await _context.Missions
                .Include(m => m.Orbital)
                .Include(m => m.Planetary)
                .FirstOrDefaultAsync(m => m.Id == mission.Id);

            if (baseEntity == null) return;

            baseEntity.Name = mission.Name;
            baseEntity.Budget = mission.Budget;
            baseEntity.Duration = mission.Duration;
            baseEntity.MissionType = (int)mission.MissionType;

            if (mission is OrbitalMission o)
            {
                if (baseEntity.Orbital == null)
                    baseEntity.Orbital = new OrbitalMissionEntity();
                baseEntity.Orbital.CurrHeight = o.CurrHeight;
                baseEntity.Orbital.TargetHeight = o.TargetHeight;
                baseEntity.Orbital.Inclination = o.Inclination;
                baseEntity.Orbital.EnergySource = (int)o.EnergySource;
                // Если раньше был Planetary – удаляем
                if (baseEntity.Planetary != null)
                    _context.PlanetaryMissions.Remove(baseEntity.Planetary);
            }
            else if (mission is PlanetaryMission p)
            {
                if (baseEntity.Planetary == null)
                    baseEntity.Planetary = new PlanetaryMissionEntity();
                baseEntity.Planetary.Planet = p.Planet;
                baseEntity.Planetary.AtmoDensity = p.AtmoDensity;
                baseEntity.Planetary.LandingPointName = p.LandingPoint.Name;
                baseEntity.Planetary.LandingPointX = p.LandingPoint.X;
                baseEntity.Planetary.LandingPointY = p.LandingPoint.Y;
                baseEntity.Planetary.LandingPointR = p.LandingPoint.R;
                if (baseEntity.Orbital != null)
                    _context.OrbitalMissions.Remove(baseEntity.Orbital);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var baseEntity = await _context.Missions.FindAsync(id);
            if (baseEntity != null)
            {
                _context.Missions.Remove(baseEntity);
                await _context.SaveChangesAsync();
            }
        }

        private Mission? ToMission(MissionBase e)
        {
            if (e.MissionType == (int)MissionT.Orbital && e.Orbital != null)
            {
                var mission = new OrbitalMission(
                    e.Name, e.Budget, e.Duration,
                    e.Orbital.CurrHeight, e.Orbital.TargetHeight, e.Orbital.Inclination,
                    (EnergySource)e.Orbital.EnergySource, _earthData);
                mission.Id = e.Id;
                return mission;
            }
            else if (e.MissionType == (int)MissionT.Planetary && e.Planetary != null)
            {
                var lp = new LandingPoint(
                    e.Planetary.LandingPointName,
                    e.Planetary.LandingPointX,
                    e.Planetary.LandingPointY,
                    e.Planetary.LandingPointR);
                var mission = new PlanetaryMission(
                    e.Name, e.Budget, e.Duration,
                    e.Planetary.Planet, e.Planetary.AtmoDensity, lp);
                mission.Id = e.Id;
                return mission;
            }
            return null;
        }
    }
}
