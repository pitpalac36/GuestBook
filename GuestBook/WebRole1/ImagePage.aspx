<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImagePage.aspx.cs" Inherits="GuestBook.WebRole.ImagePage" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <title>Image Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <img src="<%= Request.QueryString["imageUrl"] %>" alt="Image" />
        </div>
    </form>
</body>
</html>
