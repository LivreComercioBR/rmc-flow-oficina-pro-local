using System;
using System.Drawing;
using System.Windows.Forms;
using RMCFlowOficinaPro.Infrastructure;

namespace RMCFlowOficinaPro
{
    internal sealed class FirstUserForm : Form
    {
        private readonly LocalDatabase _database; private readonly TextBox _name=new TextBox(); private readonly TextBox _email=new TextBox(); private readonly TextBox _password=new TextBox(); private readonly TextBox _confirm=new TextBox();
        public FirstUserForm(LocalDatabase database){_database=database;Text="Criar administrador";StartPosition=FormStartPosition.CenterScreen;Size=new Size(480,390);MinimumSize=new Size(480,390);Build();}
        private void Build(){Controls.Add(new Label{Text="Primeiro acesso",Location=new Point(25,22),AutoSize=true,Font=new Font("Segoe UI",16,FontStyle.Bold),ForeColor=Color.FromArgb(7,55,94)});Controls.Add(new Label{Text="Defina o administrador da oficina. Esta conta terá acesso total ao sistema.",Location=new Point(27,58),Size=new Size(400,38)});Add("Nome *",_name,112);Add("E-mail *",_email,166);_password.UseSystemPasswordChar=true;_confirm.UseSystemPasswordChar=true;Add("Senha *",_password,220);Add("Confirmar senha *",_confirm,274);var save=new Button{Text="Criar administrador",Location=new Point(270,320),Size=new Size(160,32)};save.Click+=Save;Controls.Add(save);}
        private void Add(string label,TextBox field,int y){Controls.Add(new Label{Text=label,Location=new Point(25,y),AutoSize=true});field.Location=new Point(25,y+19);field.Size=new Size(405,24);Controls.Add(field);}
        private void Save(object sender,EventArgs e){if(_password.Text.Length<6){MessageBox.Show("Use uma senha com pelo menos 6 caracteres.");return;}if(_password.Text!=_confirm.Text){MessageBox.Show("A confirmação de senha não confere.");return;}try{_database.CreateUser(_name.Text,_email.Text,"Administrador",_password.Text);DialogResult=DialogResult.OK;}catch(Exception ex){MessageBox.Show(ex.Message,"Administrador",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
    }

    internal sealed class LoginForm : Form
    {
        private readonly LocalDatabase _database; private readonly TextBox _email=new TextBox();private readonly TextBox _password=new TextBox(); public SystemUser User{get;private set;}
        public LoginForm(LocalDatabase database){_database=database;Text="Entrar — RMC Flow Oficina Pro";StartPosition=FormStartPosition.CenterScreen;Size=new Size(450,340);MinimumSize=new Size(450,340);Build();}
        private void Build(){Controls.Add(new Label{Text="RMC Flow Oficina Pro",Location=new Point(25,24),AutoSize=true,Font=new Font("Segoe UI",16,FontStyle.Bold),ForeColor=Color.FromArgb(7,55,94)});Controls.Add(new Label{Text="Entre com seu e-mail e senha.",Location=new Point(27,59),AutoSize=true});Add("E-mail",_email,105);_password.UseSystemPasswordChar=true;Add("Senha",_password,164);_password.KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Enter)Enter();};var recover=new Button{Text="Recuperar acesso",Location=new Point(25,220),Size=new Size(150,32)};recover.Click+=(s,e)=>{using(var form=new RecoverAdministratorForm(_database)){form.ShowDialog(this);}};Controls.Add(recover);var enter=new Button{Text="Entrar",Location=new Point(290,220),Size=new Size(115,32)};enter.Click+=(s,e)=>Enter();Controls.Add(enter);}
        private void Add(string label,TextBox field,int y){Controls.Add(new Label{Text=label,Location=new Point(25,y),AutoSize=true});field.Location=new Point(25,y+19);field.Size=new Size(380,24);Controls.Add(field);}
        private void Enter(){User=_database.Authenticate(_email.Text,_password.Text);if(User==null){MessageBox.Show("E-mail ou senha inválidos.","Entrar",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}DialogResult=DialogResult.OK;}
    }

    internal sealed class RecoverAdministratorForm : Form
    {
        private readonly LocalDatabase _database; private readonly TextBox _request=new TextBox(); private readonly TextBox _authorization=new TextBox(); private readonly TextBox _email=new TextBox(); private readonly TextBox _password=new TextBox(); private readonly TextBox _confirm=new TextBox();
        public RecoverAdministratorForm(LocalDatabase database){_database=database;Text="Recuperar acesso do administrador";StartPosition=FormStartPosition.CenterParent;Size=new Size(540,620);MinimumSize=new Size(540,620);Build();}
        private void Build(){Controls.Add(new Label{Text="Recuperação segura do administrador",Location=new Point(24,20),AutoSize=true,Font=new Font("Segoe UI",15,FontStyle.Bold),ForeColor=Color.FromArgb(7,55,94)});Controls.Add(new Label{Text="1. Gere a solicitação e envie-a à RMC Sites.  2. Cole a autorização temporária recebida.  3. Defina a nova senha.",Location=new Point(26,54),Size=new Size(470,38)});Add("Solicitação de recuperação",_request,105,54);_request.ReadOnly=true;var generate=new Button{Text="Gerar solicitação",Location=new Point(350,166),Size=new Size(145,30)};generate.Click+=Generate;Controls.Add(generate);Add("Autorização de recuperação recebida *",_authorization,212,70);Add("E-mail do administrador *",_email,304,24);_password.UseSystemPasswordChar=true;_confirm.UseSystemPasswordChar=true;Add("Nova senha *",_password,360,24);Add("Confirmar nova senha *",_confirm,416,24);var save=new Button{Text="Validar e redefinir senha",Location=new Point(300,480),Size=new Size(195,34)};save.Click+=Save;Controls.Add(save);Controls.Add(new Label{Text="A autorização é válida por tempo limitado e é aceita uma única vez.",Location=new Point(25,530),Size=new Size(450,30),ForeColor=Color.DimGray});}
        private void Add(string label,TextBox field,int y,int height){Controls.Add(new Label{Text=label,Location=new Point(25,y),AutoSize=true});field.Location=new Point(25,y+19);field.Size=new Size(470,height);field.Multiline=height>24;Controls.Add(field);}
        private void Generate(object sender,EventArgs e){try{_request.Text=_database.CreatePasswordRecoveryRequest();_request.SelectAll();try{Clipboard.SetText(_request.Text);}catch{}MessageBox.Show("Solicitação gerada e copiada. Envie-a à RMC Sites para receber uma autorização temporária.","Solicitação gerada",MessageBoxButtons.OK,MessageBoxIcon.Information);}catch(Exception ex){MessageBox.Show(ex.Message,"Recuperar acesso",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
        private void Save(object sender,EventArgs e){if(_password.Text!=_confirm.Text){MessageBox.Show("A confirmação de senha não confere.");return;}try{var publicKey=System.IO.Path.Combine(Application.StartupPath,"keys","license-public.pem");_database.CompletePasswordRecovery(_authorization.Text,_email.Text,_password.Text,publicKey);MessageBox.Show("Senha redefinida com sucesso. Um backup foi criado antes da alteração.","Recuperação concluída",MessageBoxButtons.OK,MessageBoxIcon.Information);DialogResult=DialogResult.OK;}catch(Exception ex){MessageBox.Show(ex.Message,"Recuperar acesso",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
    }

    internal sealed class UsersForm : Form
    {
        private readonly LocalDatabase _database; private readonly long _currentUserId; private readonly DataGridView _grid=new DataGridView();
        public UsersForm(LocalDatabase database,long currentUserId){_database=database;_currentUserId=currentUserId;Text="Usuários";StartPosition=FormStartPosition.CenterParent;Size=new Size(780,510);Build();RefreshUsers();}
        private void Build(){var add=new Button{Text="Novo usuário",Location=new Point(18,16),Size=new Size(125,30)};add.Click+=(s,e)=>AddUser();Controls.Add(add);var status=new Button{Text="Ativar / desativar",Location=new Point(155,16),Size=new Size(135,30)};status.Click+=(s,e)=>ChangeStatus();Controls.Add(status);var password=new Button{Text="Redefinir senha",Location=new Point(302,16),Size=new Size(135,30)};password.Click+=(s,e)=>ResetPassword();Controls.Add(password);_grid.Location=new Point(18,60);_grid.Size=new Size(730,365);_grid.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;_grid.ReadOnly=true;_grid.AllowUserToAddRows=false;_grid.AllowUserToDeleteRows=false;_grid.SelectionMode=DataGridViewSelectionMode.FullRowSelect;_grid.MultiSelect=false;_grid.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;_grid.BackgroundColor=Color.White;Controls.Add(_grid);}
        private void RefreshUsers(){_grid.DataSource=_database.GetUsers();}
        private void AddUser(){using(var form=new NewUserForm(_database)){if(form.ShowDialog(this)==DialogResult.OK)RefreshUsers();}}
        private void ChangeStatus(){if(_grid.CurrentRow==null){MessageBox.Show("Selecione um usuário.");return;}var id=Convert.ToInt64(_grid.CurrentRow.Cells["Código"].Value);if(id==_currentUserId){MessageBox.Show("Não é possível desativar o usuário que está conectado.");return;}var active=Convert.ToString(_grid.CurrentRow.Cells["Status"].Value)!="Ativo";_database.SetUserActive(id,active);RefreshUsers();}
        private void ResetPassword(){if(_grid.CurrentRow==null){MessageBox.Show("Selecione um usuário.");return;}var user=new SystemUser{Id=Convert.ToInt64(_grid.CurrentRow.Cells["Código"].Value),Name=Convert.ToString(_grid.CurrentRow.Cells["Nome"].Value),Email=Convert.ToString(_grid.CurrentRow.Cells["E-mail"].Value)};using(var form=new ChangePasswordForm(_database,user,false)){form.ShowDialog(this);}}
    }

    internal sealed class NewUserForm : Form
    {
        private readonly LocalDatabase _database;private readonly TextBox _name=new TextBox();private readonly TextBox _email=new TextBox();private readonly ComboBox _role=new ComboBox();private readonly TextBox _password=new TextBox();
        public NewUserForm(LocalDatabase database){_database=database;Text="Novo usuário";StartPosition=FormStartPosition.CenterParent;Size=new Size(460,360);Build();}
        private void Build(){Add("Nome *",_name,22);Add("E-mail *",_email,78);_role.Items.AddRange(new object[]{"Administrador","Atendente","Mecânico"});_role.Text="Atendente";Add("Perfil *",_role,134);_password.UseSystemPasswordChar=true;Add("Senha *",_password,190);var save=new Button{Text="Salvar",Location=new Point(300,260),Size=new Size(120,32)};save.Click+=(s,e)=>{if(_password.Text.Length<6){MessageBox.Show("Use uma senha com pelo menos 6 caracteres.");return;}try{_database.CreateUser(_name.Text,_email.Text,_role.Text,_password.Text);DialogResult=DialogResult.OK;}catch(Exception ex){MessageBox.Show("Não foi possível criar o usuário. O e-mail pode já estar cadastrado.\n"+ex.Message);}};Controls.Add(save);}
        private void Add(string label,Control field,int y){Controls.Add(new Label{Text=label,Location=new Point(25,y),AutoSize=true});field.Location=new Point(25,y+19);field.Size=new Size(395,24);Controls.Add(field);}
    }

    internal sealed class ChangePasswordForm : Form
    {
        private readonly LocalDatabase _database;private readonly SystemUser _user;private readonly bool _requireCurrent;private readonly TextBox _current=new TextBox();private readonly TextBox _newPassword=new TextBox();private readonly TextBox _confirm=new TextBox();
        public ChangePasswordForm(LocalDatabase database,SystemUser user,bool requireCurrent){_database=database;_user=user;_requireCurrent=requireCurrent;Text=requireCurrent?"Alterar minha senha":"Redefinir senha";StartPosition=FormStartPosition.CenterParent;Size=new Size(450,340);Build();}
        private void Build(){Controls.Add(new Label{Text="Usuário: "+_user.Name,Location=new Point(24,20),AutoSize=true,Font=new Font("Segoe UI",11,FontStyle.Bold)});var y=65;if(_requireCurrent){_current.UseSystemPasswordChar=true;Add("Senha atual *",_current,y);y+=56;}_newPassword.UseSystemPasswordChar=true;_confirm.UseSystemPasswordChar=true;Add("Nova senha *",_newPassword,y);Add("Confirmar nova senha *",_confirm,y+56);var save=new Button{Text="Salvar senha",Location=new Point(280,250),Size=new Size(130,32)};save.Click+=Save;Controls.Add(save);}
        private void Add(string label,TextBox field,int y){Controls.Add(new Label{Text=label,Location=new Point(24,y),AutoSize=true});field.Location=new Point(24,y+19);field.Size=new Size(386,24);Controls.Add(field);}
        private void Save(object sender,EventArgs e){if(_requireCurrent&&_database.Authenticate(_user.Email,_current.Text)==null){MessageBox.Show("A senha atual está incorreta.");return;}if(_newPassword.Text!=_confirm.Text){MessageBox.Show("A confirmação de senha não confere.");return;}try{_database.ChangeUserPassword(_user.Id,_newPassword.Text);MessageBox.Show("Senha alterada com sucesso.");DialogResult=DialogResult.OK;}catch(Exception ex){MessageBox.Show(ex.Message);}}
    }

    internal sealed class ChangeEmailForm : Form
    {
        private readonly LocalDatabase _database;
        private readonly SystemUser _user;
        private readonly TextBox _currentPassword = new TextBox();
        private readonly TextBox _newEmail = new TextBox();
        private readonly TextBox _confirmEmail = new TextBox();

        public ChangeEmailForm(LocalDatabase database, SystemUser user)
        {
            _database = database; _user = user; Text = "Alterar e-mail de login";
            StartPosition = FormStartPosition.CenterParent; Size = new Size(470, 360); MinimumSize = new Size(470, 360); Build();
        }

        private void Build()
        {
            Controls.Add(new Label { Text = "E-mail atual: " + _user.Email, Location = new Point(24, 22), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(7,55,94) });
            _currentPassword.UseSystemPasswordChar = true;
            Add("Confirme sua senha atual *", _currentPassword, 65);
            Add("Novo e-mail de login *", _newEmail, 125);
            Add("Confirme o novo e-mail *", _confirmEmail, 185);
            var save = new Button { Text = "Salvar novo e-mail", Location = new Point(270, 260), Size = new Size(155, 34) };
            save.Click += Save; Controls.Add(save);
        }

        private void Add(string title, TextBox field, int y)
        {
            Controls.Add(new Label { Text = title, Location = new Point(24, y), AutoSize = true });
            field.Location = new Point(24, y + 20); field.Size = new Size(400, 25); Controls.Add(field);
        }

        private void Save(object sender, EventArgs e)
        {
            if (!string.Equals(_newEmail.Text.Trim(), _confirmEmail.Text.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("A confirmação do novo e-mail não confere.", "Alterar e-mail", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            try
            {
                _database.ChangeUserEmail(_user.Id, _currentPassword.Text, _newEmail.Text);
                _user.Email = _newEmail.Text.Trim().ToLowerInvariant();
                MessageBox.Show("E-mail de login alterado. Use o novo endereço no próximo acesso.", "Alterar e-mail", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Alterar e-mail", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
    }

    internal sealed class AuditLogForm : Form
    {
        private readonly LocalDatabase _database;private readonly DataGridView _grid=new DataGridView();
        public AuditLogForm(LocalDatabase database){_database=database;Text="Auditoria do sistema";StartPosition=FormStartPosition.CenterParent;Size=new Size(900,560);Build();}
        private void Build(){Controls.Add(new Label{Text="Últimos eventos registrados",Location=new Point(18,18),AutoSize=true,Font=new Font("Segoe UI",13,FontStyle.Bold),ForeColor=Color.FromArgb(7,55,94)});var refresh=new Button{Text="Atualizar",Location=new Point(750,14),Size=new Size(110,30)};refresh.Click+=(s,e)=>RefreshLogs();Controls.Add(refresh);_grid.Location=new Point(18,58);_grid.Size=new Size(842,440);_grid.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;_grid.ReadOnly=true;_grid.AllowUserToAddRows=false;_grid.AllowUserToDeleteRows=false;_grid.SelectionMode=DataGridViewSelectionMode.FullRowSelect;_grid.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;_grid.BackgroundColor=Color.White;Controls.Add(_grid);RefreshLogs();}
        private void RefreshLogs(){_grid.DataSource=_database.GetAuditLogs();if(_grid.Columns.Contains("Código"))_grid.Columns["Código"].Visible=false;}
    }
}
