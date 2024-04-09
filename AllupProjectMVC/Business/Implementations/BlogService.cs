using AllupProjectMVC.Areas.admin.ViewModels.Blog;
using AllupProjectMVC.Business.Interfaces;
using AllupProjectMVC.Data;
using AllupProjectMVC.Exceptions;
using AllupProjectMVC.Exceptions.BlogExceptions;
using AllupProjectMVC.Exceptions.CategoryExceptions;
using AllupProjectMVC.Extension;
using AllupProjectMVC.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace AllupProjectMVC.Business.Implementations
{
    public class BlogService : IBlogService
    {
        private readonly ADbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public BlogService(ADbContext context,
                           IMapper mapper,
                           IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }
        public async Task<List<BlogVM>> GetAllAsync()
        {
            return _mapper.Map<List<BlogVM>>(await _context.Blogs.ToListAsync());
        }

        public async Task<BlogVM> GetByIdAsync(int id)
        {
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);
            return _mapper.Map<BlogVM>(blog);
        }

        public async Task CreateAsync(BlogCreateVM request)
        {
            string fileName = $"{Guid.NewGuid()}-{request.Photo.FileName}";
            string path = _env.GetFilePath("uploads/blogs", fileName);

            var data = _mapper.Map<Blog>(request);

            data.Image = fileName;

            await _context.Blogs.AddAsync(data);
            await _context.SaveChangesAsync();
            await request.Photo.SaveFileAsync(path);
        }


        public async Task DeleteAsync(int id)
        {
            Blog blog = await _context.Blogs.Where(m => m.Id == id).FirstOrDefaultAsync();
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            string path = _env.GetFilePath("uploads/blogs", blog.Image);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

        }


        public async Task UpdateAsync(BlogUpdateVM request)
        {
            string fileName;

            if (request.Photo is not null)
            {
                string oldPath = _env.GetFilePath("uploads/blogs", request.Image);
                fileName = $"{Guid.NewGuid()}-{request.Photo.FileName}";
                string newPath = _env.GetFilePath("uploads/blogs", fileName);

                if (File.Exists(oldPath))
                {
                    File.Delete(oldPath);
                }

                await request.Photo.SaveFileAsync(newPath);

            }
            else
            {
                fileName = request.Image;
            }

            Blog dbBlog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == request.Id);


            _mapper.Map(request, dbBlog);

            dbBlog.Image = fileName;

            _context.Blogs.Update(dbBlog);

            await _context.SaveChangesAsync();
        }

    }
}
