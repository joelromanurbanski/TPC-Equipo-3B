<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InicioSesion.aspx.cs" Inherits="tp_c_equipo_3B.InicioSesion" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Iniciar Sesión</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f5f6fa; display: flex; justify-content: center; align-items: center; height: 100vh; margin: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .login-card { background: white; padding: 2rem; border-radius: 1rem; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1); width: 100%; max-width: 400px; }
        .login-icon { font-size: 3rem; color: #0d6efd; margin-bottom: 1rem; text-align: center; }
        .login-header { text-align: center; margin-bottom: 2rem; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-card">
            <div class="login-header">
                <div class="login-icon">🔐</div>
                <h3 class="fw-bold">Bienvenido</h3>
                <p class="text-muted">Ingresa tus credenciales</p>
            </div>

            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger text-center p-2 mb-3">
                <asp:Label ID="lblError" runat="server" />
            </asp:Panel>

            <div class="mb-3">
                <label class="form-label">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="admin@admin.com" />
            </div>
            
            <div class="mb-4">
                <label class="form-label">Contraseña</label>
                <asp:TextBox ID="txtPass" runat="server" CssClass="form-control" TextMode="Password" />
            </div>

            <div class="d-grid">
                <asp:Button ID="btnLogin" runat="server" Text="Ingresar" CssClass="btn btn-primary btn-lg" OnClick="btnLogin_Click" />
            </div>
        </div>
    </form>
</body>
</html>