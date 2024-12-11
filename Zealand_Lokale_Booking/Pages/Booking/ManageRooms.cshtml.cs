﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Zealand_Lokale_Booking.Pages.Booking
{
    public class ManageRoomsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ManageRoomsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Room> Rooms { get; set; }
        public string Filter { get; set; } // Filtrering af lokaler

        public void OnGet(string filter = "all")
        {
            Filter = filter;

            // Filtrering af lokaler baseret på status
            if (filter == "booked")
            {
                Rooms = _context.Rooms
                    .Include(r => r.BookedByUser)
                    .Where(r => r.IsBooked)
                    .ToList();
            }
            else if (filter == "available")
            {
                Rooms = _context.Rooms
                    .Where(r => !r.IsBooked)
                    .ToList();
            }
            else // "all"
            {
                Rooms = _context.Rooms
                    .Include(r => r.BookedByUser)
                    .ToList();
            }
        }

        public async Task<IActionResult> OnPostCancelBookingAsync(int roomId)
        {
            var room = _context.Rooms.Include(r => r.BookedByUser).FirstOrDefault(r => r.RoomId == roomId);

            if (room == null || !room.IsBooked)
            {
                TempData["ErrorMessage"] = "Lokalet har ingen aktiv booking.";
                return RedirectToPage(new { filter = Filter });
            }

            // Fjern booking fra lokalet
            room.IsBooked = false;
            var bookedUserId = room.BookedByUserId;
            room.BookedByUserId = null;

            // Opdater bookingstatus
            var booking = _context.Bookings.FirstOrDefault(b => b.RoomId == roomId && b.Status == "Active");
            if (booking != null)
            {
                booking.Status = "Cancelled";
                booking.IsActive = false;
            }

            // Opret notifikation
            if (bookedUserId.HasValue)
            {
                var notification = new Notification
                {
                    UserId = bookedUserId.Value,
                    Message = $"Din booking for lokalet \"{room.RoomName}\" er blevet annulleret.",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Bookingen for lokalet \"{room.RoomName}\" blev annulleret, og brugeren fik en notifikation.";
            return RedirectToPage(new { filter = Filter });
        }
    }
}












