using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSAD_Project.Models.ViewModels.ShoppingCart
{
    public class ShoppingCartViewModel
    {
        public ShoppingCartViewModel() { }

        public ShoppingCartViewModel(Data.ShoppingCart rowDTO)
        {
            this.ShoppingCartId = rowDTO.Id;
            this.UserId = rowDTO.UserId;
            this.SessionId = rowDTO.SessionId;
            this.Session = new ViewModels.Session.SessionViewModel(rowDTO.Session);
        }


        public int ShoppingCartId { get; set; }
        public int UserId { get; set; }
        public int SessionId { get; set; }
        public bool IsSelected { get; set; }
        public ViewModels.Session.SessionViewModel Session { get; set; }
    }
}