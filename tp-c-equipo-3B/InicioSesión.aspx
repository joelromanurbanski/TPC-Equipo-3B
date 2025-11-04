<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InicioSesión.aspx.cs" Inherits="tp_c_equipo_3B.InicioSesión" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Iniciar Sesión</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f5f6fa;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }

        .login-container {
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0px 2px 8px rgba(0, 0, 0, 0.1);
            width: 350px;
            text-align: center;
            padding: 30px 25px;
        }

        .login-icon {
            background-color: #e7f0ff;
            color: #004aad;
            font-size: 28px;
            border-radius: 50%;
            width: 60px;
            height: 60px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 15px;
        }

        h2 {
            color: #333;
            margin-bottom: 5px;
        }

        p {
            color: #666;
            font-size: 14px;
            margin-bottom: 25px;
        }

        .form-control {
            width: 100%;
            padding: 10px;
            margin-bottom: 15px;
            border-radius: 6px;
            border: 1px solid #ccc;
            font-size: 14px;
        }

        .btn-login {
            width: 100%;
            padding: 10px;
            background-color: #004aad;
            color: white;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            font-size: 15px;
            font-weight: bold;
        }

        .btn-login:hover {
            background-color: #003c8a;
        }

        .forgot-password {
            display: block;
            margin-top: 15px;
            font-size: 13px;
            color: #004aad;
            text-decoration: none;
        }

        .forgot-password:hover {
            text-decoration: underline;
        }

        .footer {
            margin-top: 30px;
            font-size: 12px;
            color: #aaa;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <div class="login-icon">💻</div>
            <h2>Bienvenido</h2>
            <p>Ingresa tus credenciales para acceder a tu cuenta.</p>

            <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Ingresa tu nombre de usuario"></asp:TextBox>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Ingresa tu contraseña"></asp:TextBox>

            <asp:Button ID="btnLogin" runat="server" Text="Iniciar Sesión" CssClass="btn-login" OnClick="btnLogin_Click" />

            <a href="#" class="forgot-password">¿Olvidaste tu contraseña?</a>

            <div class="footer">
                © 2024 Mi Negocio S.A.
            </div>
        </div>
    </form>
</body>
</html>
