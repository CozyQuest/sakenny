﻿namespace sakenny.Application.DTO
{
    public class PagedResultDTO<T>
    {
         public List<T> Items { get; set; }
         public int TotalCount { get; set; }
    }
}
