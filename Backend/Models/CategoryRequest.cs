﻿namespace Backend.Models
{
    public class CategoryRequest
    {
        public int Id { get; set; }
        public int PublicAccess {  get; set; }
        public string Type { get; set; }
    }
}
