using HallBooking.Data;
using HallBooking.Entities;
using Microsoft.EntityFrameworkCore;


namespace HallBooking.Services
{
    public class MemberService
    {

        public async Task<int> CreateMemberAsync(string name, string email)
        {
            using var context = new AppDbContext();
            var member = new Member
            { Name = name, Email = email };

            context.Members.Add(member);
            await context.SaveChangesAsync();

            return member.Id;
        }

        public async Task UpdateMemberAsync(int id, string name, string email)
        {
            using var context = new AppDbContext();
            var member = await context.Members
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
                throw new InvalidOperationException("Användare hittades inte");

            member.Name = name;
            member.Email = email;
            await context.SaveChangesAsync();
            
        }

        public async Task UpdateEmailAsync(int id, string email)
        {
            using var context = new AppDbContext();
            var member = await context.Members
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
                throw new InvalidOperationException("Användare hittades inte");

            member.Email = email;
            await context.SaveChangesAsync();
        }

        public async Task DeleteMemberAsync(int id)
        {
            var context = new AppDbContext();
            var member = await context.Members
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
                throw new InvalidOperationException("Användare hittades inte");

            context.Members.Remove(member);
            await context.SaveChangesAsync();
        }

        public async Task<List<MemberListView>> GetMembersAsync() 
        {
            using var context = new AppDbContext();
            return await context.Members
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .Select(m => new MemberListView(
                    m.Id,
                    m.Name,
                    m.Email
                    ))
                .ToListAsync();
        }

        public async Task<MemberDetailsView?> GetMemberByIdAsync(int memberId)
        {
            using var context = new AppDbContext();
            return await context.Members
                .AsNoTracking()
                .Where(m => m.Id == memberId)
                .Select(m => new MemberDetailsView(
                    m.Id, m.Name, m.Email, m.CreatedAt))
                .FirstOrDefaultAsync();
        }

        public async Task<MemberDetailsView?> GetMemberByEmailAsync(string email)
        {
            using var context = new AppDbContext();
            return await context.Members
                .AsNoTracking()
                .Where(m => m.Email == email)
                .Select(m => new MemberDetailsView(m.Id, m.Name, m.Email, m.CreatedAt))
                .FirstOrDefaultAsync();
        }

        public record MemberListView(
            int Id,
            string Name,
            string Email
            );

        public record MemberDetailsView(
            int Id,
            string Name, 
            string Email, 
            DateTime CreatedAt
            );
    }
}
