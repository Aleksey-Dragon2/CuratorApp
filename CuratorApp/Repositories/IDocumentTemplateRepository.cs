using CuratorApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorApp.Repositories
{
    public interface IDocumentTemplateRepository
    {
        Task<List<DocumentTemplate>> GetAllAsync();
        Task<DocumentTemplate> CreateAsync(DocumentTemplate template);
        Task<List<TemplateKeyword>> GetKeywordsAsync(int templateId);
        Task SaveKeywordsAsync(int templateId, List<string> placeholders);
        Task<List<DocumentTemplate>> GetByTypeAsync(TemplateType templateType);
        Task UpdateAsync(DocumentTemplate template);
        Task DeleteAsync(int id);
    }

}
