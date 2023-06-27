﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GuestBook.WebRole._Default" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Guest Book</title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
    <link rel="stylesheet" type="text/css" href="Content/bootstrap.css">
    <link rel="stylesheet" type="text/css" href="Content/Site.css">
    <script src="//code.jquery.com/jquery-1.11.0.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/js/bootstrap.min.js" integrity="sha384-wfSDF2E50Y2D1uUdj0O3uMBJnjuUD4Ih7YwaYd1iqfktj0Uod8GCExl3Og8ifwB6" crossorigin="anonymous"></script>
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1"
            runat="server">
        </asp:ScriptManager>

        <nav class="navbar navbar-light bg-dark rounded-0 pt-1">
            <h2 class="text-light">Guest Book</h2>
        </nav>

        <div class="jumbotron">

            <div>
                <div class="form-group row">
                    <div class="col-2 col-md-2 col-lg-1 pr-0">
                        <label for="NameLabel">Name:</label>
                    </div>
                    <div class="col-10 col-md-5 col-lg-5 pl-0">
                        <asp:TextBox
                            ID="NameTextBox"
                            runat="server"
                            class="form-control" />
                        <asp:RequiredFieldValidator
                            ID="NameRequiredValidator"
                            runat="server"
                            ControlToValidate="NameTextBox"
                            Text="*" />
                    </div>
                </div>

                <div class="form-group row">
                    <div class="col-2 col-md-2 col-lg-1 pr-0">
                        <label for="MessageLabel">Message:</label>
                    </div>
                    <div class="col-10 col-md-5 col-lg-5 pl-0">
                        <asp:TextBox
                            ID="MessageTextBox"
                            runat="server"
                            class="form-control"
                            TextMode="MultiLine"
                            Rows="5"
                            ToolTip="Enter comment here" />
                    </div>
                </div>

                <div class="form-group row">
                    <div class="col-2 col-md-2 col-lg-1 pr-0">
                        <label for="MessageLabel">Picture:</label>
                    </div>
                    <div class="col-10 col-md-5 col-lg-5 pl-0">
                        <asp:FileUpload
                            ID="FileUpload1"
                            runat="server"></asp:FileUpload>
                    </div>
                </div>

                <div class="row">
                    <div class="col-12 col-md-7 col-lg-6 text-center">
                        <asp:Button ID="SignButton"
                            Text="Submit"
                            OnClick="SignButton_Click"
                            runat="server"
                            class="btn btn-info"></asp:Button>
                    </div>
                </div>

            </div>

            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
            <div class="py-5 mt-3">
                <h3>Comments</h3>

                <asp:DataList ID="DataList1"
                    runat="server"
                    DataSourceID="ObjectDataSource1"
                    BorderColor="black"
                    CellPadding="5"
                    CellSpacing="5"
                    RepeatDirection="Vertical"
                    RepeatLayout="Flow"
                    RepeatColumns="0"
                    BorderWidth="0"
                    Style="margin-top: 1em;">

                    <ItemTemplate>
                        <div class="card mw-100">
                            <div class="row">
                                <div class="col-auto">
                                    <asp:ImageButton ID="Image" class="card-img-left" ImageUrl='<%# DataBinder.Eval(Container.DataItem, "ThumbnailUrl") %>' CausesValidation="false" runat="server" FullImageUrl='<%# DataBinder.Eval(Container.DataItem, "PhotoUrl") %>' OnClick="Image_Click1" />
                                </div>
                                <div class="col">
                                    <div class="card-block px-2 py-3">
                                        <h4 class="card-title">
                                            <strong>
                                                <%# DataBinder.Eval(Container.DataItem,"GuestName") %>
                                            </strong>
                                        </h4>
                                        <p class="card-text">
                                            <%# DataBinder.Eval(Container.DataItem,"Message") %>
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </ItemTemplate>

                </asp:DataList>
                <asp:Timer 
                    ID="Timer1" 
                    runat="server"
                    Interval="3000"
                    OnTick="Timer1_Tick">
                </asp:Timer>
                </ContentTemplate>
            </asp:UpdatePanel>
            </div>
            <asp:ObjectDataSource 
               ID="ObjectDataSource1"
               runat="server" 
               DataObjectTypeName="GuestBook.Data.Entry"
               SelectMethod="GetEntries" 
               TypeName="GuestBook.Data.DataSource">
            </asp:ObjectDataSource>

        </div>

        <div class="modal fade" id="imageModal" role="dialog" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <asp:UpdatePanel ID="upModal" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="modal-content">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <div class="modal-body img-modal">
                                <asp:Image ID="ImageFull" runat="server" />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>

    </form>

</body>
</html>
