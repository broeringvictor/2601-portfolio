using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TestYaml
{
    public class BlogPost
    {
        public string Title { get; set; } = string.Empty;
        public string Lead { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public DateTime PublishedAt { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
    }

    class Program
    {
        static void Main(string[] args)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            var markdown = @"---
title: My Test Post
lead: This is a lead
isPublished: true
publishedAt: 2024-02-08
slug: my-test-post
author: Victor
excerpt: Summary here
tags:
  - Tag1
  - Tag2
---
# My Title
Body content";

            var markDoc = Markdown.Parse(markdown, pipeline);
            var yamlMetadata = markDoc.OfType<YamlFrontMatterBlock>().FirstOrDefault();

            if (yamlMetadata != null)
            {
                var yamlContent = yamlMetadata.Lines.ToString();
                Console.WriteLine("YAML Content:");
                Console.WriteLine(yamlContent);

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .IgnoreUnmatchedProperties()
                    .Build();
                
                var post = deserializer.Deserialize<BlogPost>(yamlContent);
                
                Console.WriteLine($"Title: {post.Title}");
                Console.WriteLine($"Tags: {string.Join(", ", post.Tags)}");
                Console.WriteLine($"PublishedAt: {post.PublishedAt}");
            }
            else
            {
                Console.WriteLine("No YAML metadata found.");
            }
        }
    }
}
