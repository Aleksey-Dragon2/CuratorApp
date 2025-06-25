using CuratorApp.Data;
using CuratorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CuratorApp.Repositories
{
    public class DocumentTemplateRepository : IDocumentTemplateRepository
    {
        private readonly ApplicationContext _context;

        public DocumentTemplateRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<DocumentTemplate>> GetAllAsync()
        {
            return await _context.DocumentTemplates.AsNoTracking().ToListAsync();
        }

        public async Task<DocumentTemplate> CreateAsync(DocumentTemplate template)
        {
            _context.DocumentTemplates.Add(template);
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task<List<TemplateKeyword>> GetKeywordsAsync(int templateId)
        {
            return await _context.TemplateKeywords
                .Where(k => k.DocumentTemplateId == templateId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task SaveKeywordsAsync(int templateId, List<string> placeholders)
        {
            var existing = await _context.TemplateKeywords
                .Where(k => k.DocumentTemplateId == templateId)
                .ToListAsync();

            _context.TemplateKeywords.RemoveRange(existing);

            foreach (var p in placeholders.Distinct())
            {
                _context.TemplateKeywords.Add(new TemplateKeyword
                {
                    DocumentTemplateId = templateId,
                    Placeholder = p
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DocumentTemplate template)
        {
            _context.DocumentTemplates.Update(template);
            await _context.SaveChangesAsync();
        }
        public async Task<List<DocumentTemplate>> GetByTypeAsync(TemplateType templateType)
        {
            return await _context.DocumentTemplates
                .Where(t => t.TemplateType == templateType)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var template = await _context.DocumentTemplates.FindAsync(id);
            if (template != null)
            {
                _context.DocumentTemplates.Remove(template);
                await _context.SaveChangesAsync();
            }
        }
    }

}
