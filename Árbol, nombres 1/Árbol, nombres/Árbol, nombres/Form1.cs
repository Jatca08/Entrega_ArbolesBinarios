using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Árbol__nombres
{
    public partial class Form1 : Form
    {
        // Modelo de datos (árbol binario de nombres)
        private sealed class NameNode
        {
            public string Name { get; }
            public NameNode? Left { get; set; }
            public NameNode? Right { get; set; }

            public NameNode(string name) => Name = name;
            public int ChildrenCount => (Left is null ? 0 : 1) + (Right is null ? 0 : 1);
            public override string ToString() => Name;
        }

        private enum InsertSide { Auto, Left, Right }

        private sealed class NameTree
        {
            private readonly HashSet<string> _names = new(StringComparer.OrdinalIgnoreCase); //lista de
            public NameNode? Root { get; private set; }

            public bool IsEmpty => Root is null;

            public bool Contains(string name) => _names.Contains(name);

            public bool TryAdd(string name, NameNode? parent, InsertSide side, out string error, out NameNode added)
            {
                added = null!;
                error = string.Empty;

                if (IsEmpty)
                {
                    if (parent is not null)
                    {
                        error = "El árbol está vacío. El primer nombre debe ser la raíz (no selecciones padre).";
                        return false;
                    }
                    if (_names.Contains(name))
                    {
                        error = "El nombre ya existe en el árbol.";
                        return false;
                    }
                    Root = new NameNode(name);
                    _names.Add(name);
                    added = Root;
                    return true;
                }

                if (parent is null)
                {
                    error = "Debes seleccionar un nodo padre para insertar un hijo.";
                    return false;
                }

                if (_names.Contains(name))
                {
                    error = "El nombre ya existe en el árbol.";
                    return false;
                }

                if (parent.ChildrenCount >= 2)
                {
                    error = $"El nodo \"{parent.Name}\" ya tiene 2 hijos.";
                    return false;
                }

                NameNode newNode = new(name);

                if (side == InsertSide.Left)
                {
                    if (parent.Left is not null)
                    {
                        error = $"El hijo izquierdo de \"{parent.Name}\" ya está ocupado.";
                        return false;
                    }
                    parent.Left = newNode;
                }
                else if (side == InsertSide.Right)
                {
                    if (parent.Right is not null)
                    {
                        error = $"El hijo derecho de \"{parent.Name}\" ya está ocupado.";
                        return false;
                    }
                    parent.Right = newNode;
                }
                else // Auto
                {
                    if (parent.Left is null) parent.Left = newNode;
                    else if (parent.Right is null) parent.Right = newNode;
                    else
                    {
                        error = $"El nodo \"{parent.Name}\" ya tiene 2 hijos.";
                        return false;
                    }
                }

                _names.Add(name);
                added = newNode;
                return true;
            }

            public NameNode? FindByName(string name)
            {
                if (Root is null) return null;
                var stack = new Stack<NameNode>();
                stack.Push(Root);

                while (stack.Count > 0)
                {
                    var n = stack.Pop();
                    if (string.Equals(n.Name, name, StringComparison.OrdinalIgnoreCase))
                        return n;

                    if (n.Right is not null) stack.Push(n.Right);
                    if (n.Left is not null) stack.Push(n.Left);
                }
                return null;
            }

            // Recorridos: Preorden, Inorden, Postorden
            public IEnumerable<string> PreOrder()
            {
                foreach (var n in PreOrderNodes(Root)) yield return n.Name;

                static IEnumerable<NameNode> PreOrderNodes(NameNode? node)
                {
                    if (node is null) yield break;
                    yield return node;
                    foreach (var x in PreOrderNodes(node.Left)) yield return x;
                    foreach (var x in PreOrderNodes(node.Right)) yield return x;
                }
            }

            public IEnumerable<string> InOrder()
            {
                foreach (var n in InOrderNodes(Root)) yield return n.Name;

                static IEnumerable<NameNode> InOrderNodes(NameNode? node)
                {
                    if (node is null) yield break;
                    foreach (var x in InOrderNodes(node.Left)) yield return x;
                    yield return node;
                    foreach (var x in InOrderNodes(node.Right)) yield return x;
                }
            }

            public IEnumerable<string> PostOrder()
            {
                foreach (var n in PostOrderNodes(Root)) yield return n.Name;

                static IEnumerable<NameNode> PostOrderNodes(NameNode? node)
                {
                    if (node is null) yield break;
                    foreach (var x in PostOrderNodes(node.Left)) yield return x;
                    foreach (var x in PostOrderNodes(node.Right)) yield return x;
                    yield return node;
                }
            }

            // Ancho por niveles (se mantiene por si lo usas en otra parte)
            public IEnumerable<string> GetAllNames()
            {
                if (Root is null) yield break;
                var queue = new Queue<NameNode>();
                queue.Enqueue(Root);
                while (queue.Count > 0)
                {
                    var n = queue.Dequeue();
                    yield return n.Name;
                    if (n.Left is not null) queue.Enqueue(n.Left);
                    if (n.Right is not null) queue.Enqueue(n.Right);
                }
            }
        }

        // Controles de UI
        private TextBox txtNombre = null!;
        private RadioButton rbAuto = null!;
        private RadioButton rbIzq = null!;
        private RadioButton rbDer = null!;
        private Button btnAgregar = null!;
        private Button btnBuscar = null!;
        private Button btnOrdenados = null!;
        private Button btnVaciar = null!;
        private Button btnExpandir = null!;
        private Button btnContraer = null!;
        private Button btnAsignacion = null!;
        private TreeView tvArbol = null!;
        private ListBox lstOrdenados = null!;
        private Label lblEstado = null!;
        private Label lblSeleccion = null!;
        // NUEVO: opciones de recorrido
        private RadioButton rbPreorden = null!;
        private RadioButton rbInorden = null!;
        private RadioButton rbPostorden = null!;

        // Estado
        private readonly NameTree _tree = new();
        private static readonly Regex NombreValidoRegex = new(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ ]{1,50}$", RegexOptions.Compiled);

        public Form1()
        {
            InitializeComponent();
            BuildUi();
            WireEvents();
        }


        //todo lo que es construir la ui
        private void BuildUi()
        {
            Text = "Árbol de Nombres (nodos)";
            Width = 980;
            Height = 620;
            StartPosition = FormStartPosition.CenterScreen;

            var lblTitulo = new Label
            {
                Text = "Ejercicio 2 – Árbol de Nombres",
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold),
                Top = 10,
                Left = 12
            };

            var lblNombre = new Label { Text = "Nombre:", AutoSize = true, Top = 50, Left = 12 };
            txtNombre = new TextBox { Top = lblNombre.Top - 3, Left = 80, Width = 260, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

            var grpPos = new GroupBox
            {
                Text = "Ubicación del nuevo nodo (respecto al padre seleccionado)",
                Top = 85,
                Left = 12,
                Width = 540,
                Height = 65,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            rbAuto = new RadioButton { Text = "Automático (primera posición libre)", Left = 12, Top = 28, Checked = true, AutoSize = true };
            rbIzq = new RadioButton { Text = "Izquierdo", Left = 260, Top = 28, AutoSize = true };
            rbDer = new RadioButton { Text = "Derecho", Left = 360, Top = 28, AutoSize = true };
            grpPos.Controls.AddRange(new Control[] { rbAuto, rbIzq, rbDer });

            btnAgregar = new Button { Text = "Insertar", Top = grpPos.Bottom + 8, Left = 12, Width = 120 };
            btnBuscar = new Button { Text = "Buscar", Top = grpPos.Bottom + 8, Left = 140, Width = 120 };
            btnOrdenados = new Button { Text = "Mostrar recorrido", Top = grpPos.Bottom + 8, Left = 268, Width = 160 };
            btnVaciar = new Button { Text = "Vaciar árbol", Top = grpPos.Bottom + 8, Left = 436, Width = 120 };

            lblSeleccion = new Label
            {
                Text = "Padre seleccionado: (ninguno)",
                AutoSize = true,
                Top = btnAgregar.Bottom + 10,
                Left = 12
            };

            tvArbol = new TreeView
            {
                Top = lblSeleccion.Bottom + 8,
                Left = 12,
                Width = 540,
                Height = 420,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            btnExpandir = new Button { Text = "Expandir todo", Top = tvArbol.Bottom + 6, Left = 12, Width = 120, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            btnContraer = new Button { Text = "Contraer todo", Top = tvArbol.Bottom + 6, Left = 140, Width = 120, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            btnAsignacion = new Button { Text = "Ver asignación", Top = tvArbol.Bottom + 6, Left = 268, Width = 160, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };

            // Lado derecho: selección de recorrido + lista
            var lblOrdenados = new Label
            {
                Text = "Seleccione el tipo de recorrido:",
                AutoSize = true,
                Top = 50,
                Left = 580
            };

            var grpRecorrido = new GroupBox
            {
                Text = "Tipo de recorrido",
                Top = lblOrdenados.Bottom + 6,
                Left = 580,
                Width = 370,
                Height = 60,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            rbPreorden = new RadioButton { Text = "Preorden", Left = 12, Top = 25, AutoSize = true };
            rbInorden = new RadioButton { Text = "Inorden", Left = 120, Top = 25, AutoSize = true, Checked = true };
            rbPostorden = new RadioButton { Text = "Postorden", Left = 220, Top = 25, AutoSize = true };
            grpRecorrido.Controls.AddRange(new Control[] { rbPreorden, rbInorden, rbPostorden });

            lstOrdenados = new ListBox
            {
                Top = grpRecorrido.Bottom + 6,
                Left = 580,
                Width = 370,
                Height = 505,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };

            lblEstado = new Label
            {
                Text = "Listo.",
                AutoSize = true,
                Top = btnExpandir.Bottom + 10,
                Left = 280,
                ForeColor = System.Drawing.Color.DimGray,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            Controls.AddRange(new Control[]
            {
                lblTitulo, lblNombre, txtNombre, grpPos, btnAgregar, btnBuscar, btnOrdenados, btnVaciar,
                lblSeleccion, tvArbol, btnExpandir, btnContraer, btnAsignacion, lblEstado,
                lblOrdenados, grpRecorrido, lstOrdenados
            });
        }

        private void WireEvents() //conectar eventos a todos los recursos de la UI
        {
            Load += Form1_Load; //se ejecuta cuando se carga el formulario
            btnAgregar.Click += (_, __) => Insertar(); // _, __ son parámetros integrados en los recursos que no se usan
            btnBuscar.Click += (_, __) => Buscar(); // usar _, __ es como para rellenar ya que solo nos interesa el evento del inicio
            btnOrdenados.Click += (_, __) => MostrarOrdenados();
            btnVaciar.Click += (_, __) => VaciarArbol();
            btnExpandir.Click += (_, __) => tvArbol.ExpandAll();
            btnContraer.Click += (_, __) => tvArbol.CollapseAll();
            btnAsignacion.Click += (_, __) => MostrarAsignacion();
            tvArbol.AfterSelect += (_, __) => ActualizarSeleccionLabel();
            tvArbol.NodeMouseDoubleClick += (_, e) =>
            {
                if (e.Node.IsExpanded) e.Node.Collapse(); //si el nodo esta expandido, colapsarlo
                else e.Node.Expand(); //si no, expandirlo
            };
            txtNombre.KeyDown += (_, e) => //evento para detectar cuando se presiona una tecla
            {
                if (e.KeyCode == Keys.Enter) ///si la tecla presionada es Enter
                {
                    Insertar();
                    e.SuppressKeyPress = true; //evita el sonido de la ventana al presionar Enter
                }
            };
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            // No-op on load; la UI se construye en el constructor.
        }

        // Acciones
        private void Insertar()
        {
            string nombre = txtNombre.Text.Trim(); //elimina espacios en blanco al inicio y al final

            if (!_tree.IsEmpty && tvArbol.SelectedNode is null)
            {
                Info("Selecciona un nodo en el árbol para usar como padre, o vacía el árbol para crear una nueva raíz.");
                return;
            }

            if (!ValidarNombre(nombre, out var errorNombre)) //pues lo que dice
            {
                Error(errorNombre);
                return;
            }

            var padre = tvArbol.SelectedNode?.Tag as NameNode; //establecemos el tipo de etiqueta para el nodo para poner límite mas adelante
            var side = rbIzq.Checked ? InsertSide.Left : rbDer.Checked ? InsertSide.Right : InsertSide.Auto; //hijos, si es izquierdo, derecho o automático

            if (_tree.TryAdd(nombre, padre, side, out string error, out var agregado)) //si se pudo agregar
            {
                RefrescarTreeView(seleccionar: agregado);
                txtNombre.Clear(); //limpia el textbox
                Ok($"Insertado \"{agregado.Name}\"{(padre is null ? " como raíz." : $" como hijo de \"{padre.Name}\".")}"); //mensaje de éxito
            }
            else
            {
                Error(error);
            }
        }

        private void Buscar()
        {
            if (_tree.IsEmpty)
            {
                Info("El árbol está vacío.");
                return;
            }

            string nombre = txtNombre.Text.Trim();
            if (!ValidarNombreParaBusqueda(nombre, out var err))
            {
                Error(err);
                return;
            }

            var nodo = _tree.FindByName(nombre);
            if (nodo is null)
            {
                Info($"\"{nombre}\" no se encontró.");
                return;
            }

            // Seleccionar en el TreeView
            if (TrySelectNodeByTag(tvArbol.Nodes, nodo, out var treeNode))
            {
                tvArbol.SelectedNode = treeNode;
                treeNode.EnsureVisible();
                Ok($"Encontrado: \"{nodo.Name}\".");
            }
            else
            {
                // Si por alguna razón el TV no estaba sincronizado
                RefrescarTreeView(seleccionar: nodo);
                Ok($"Encontrado: \"{nodo.Name}\".");
            }
        }

        private void MostrarOrdenados()
        {
            if (_tree.IsEmpty)
            {
                lstOrdenados.Items.Clear();
                Info("El árbol está vacío.");
                return;
            }

            IEnumerable<string> recorrido = //seleccionar recorrido según opción
                rbPreorden.Checked ? _tree.PreOrder() :
                rbPostorden.Checked ? _tree.PostOrder() :
                _tree.InOrder(); // por defecto, inorden

            var resultado = recorrido.ToArray();

            lstOrdenados.BeginUpdate(); //evita parpadeo al actualizar
            lstOrdenados.Items.Clear(); //limpia la lista
            lstOrdenados.Items.AddRange(resultado); //agrega los nombres ordenados a la lista
            lstOrdenados.EndUpdate(); //termina la actualización

            var tipo = rbPreorden.Checked ? "preorden" : rbPostorden.Checked ? "postorden" : "inorden";
            Ok($"Mostrando {resultado.Length} nombre(s) en {tipo}.");
        }

        private void VaciarArbol()
        {
            tvArbol.Nodes.Clear();
            lstOrdenados.Items.Clear();
            //crear nuevo árbol 
            typeof(Form1).GetField("_tree", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)! //binding flags para acceder al campo privado y de instancia para crear un nuevo árbol
                .SetValue(this, new NameTree());
            lblSeleccion.Text = "Padre seleccionado: (ninguno)";
            Info("Árbol vaciado.");
        }

        //utilidades UI
        private void RefrescarTreeView(NameNode? seleccionar = null)
        {
            tvArbol.BeginUpdate();
            tvArbol.Nodes.Clear();

            if (!_tree.IsEmpty && _tree.Root is not null) //si el árbol no está vacío
            {
                var rootNode = BuildTreeNode(_tree.Root); //construir el nodo raíz
                tvArbol.Nodes.Add(rootNode); //agregar el nodo raíz al TreeView

                if (seleccionar is not null && TrySelectNodeByTag(tvArbol.Nodes, seleccionar, out var selected))
                {
                    tvArbol.SelectedNode = selected;
                    selected.EnsureVisible();
                }
                else
                {
                    tvArbol.ExpandAll();
                }
            }
            tvArbol.EndUpdate();
            ActualizarSeleccionLabel();
        }

        private TreeNode BuildTreeNode(NameNode n) //construir un nodo del TreeView a partir de un NameNode
        {
            var tn = new TreeNode(n.Name) { Tag = n };
            if (n.Left is not null) tn.Nodes.Add(BuildTreeNode(n.Left)); //agregar hijo izquierdo recursivamente
            if (n.Right is not null) tn.Nodes.Add(BuildTreeNode(n.Right)); //agregar hijo derecho recursivamente
            return tn;
        }

        private static bool TrySelectNodeByTag(TreeNodeCollection nodes, NameNode target, out TreeNode? found) //buscar un nodo en el TreeView por su etiqueta (Tag)
        {
            foreach (TreeNode node in nodes) //recorrer todos los nodos
            {
                if (ReferenceEquals(node.Tag, target)) //si la etiqueta del nodo es igual al objetivo
                {
                    found = node; //asignar el nodo encontrado
                    return true;
                }
                if (TrySelectNodeByTag(node.Nodes, target, out found))
                    return true;
            }
            found = null;
            return false;
        }

        private void ActualizarSeleccionLabel() //actualizar la etiqueta que muestra el nodo padre seleccionado
        {
            if (tvArbol.SelectedNode?.Tag is NameNode n) //si el nodo seleccionado no es nulo y su etiqueta es un NameNode
                lblSeleccion.Text = $"Padre seleccionado: {n.Name} (hijos: {n.ChildrenCount}/2)"; //mostrar nombre y número de hijos
            else
                lblSeleccion.Text = "Padre seleccionado: (ninguno)";
        }

        private static bool ValidarNombre(string nombre, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(nombre))
            {
                error = "Ingresa un nombre.";
                return false;
            }
            if (!NombreValidoRegex.IsMatch(nombre))
            {
                error = "El nombre solo puede contener letras y espacios (máx. 50 caracteres).";
                return false;
            }
            return true;
        }

        private static bool ValidarNombreParaBusqueda(string nombre, out string error)
        {
            // En búsqueda permitimos 1..50 letras/espacios, igual que inserción.
            return ValidarNombre(nombre, out error);
        }

        private void Ok(string msg) => SetEstado(msg, System.Drawing.Color.ForestGreen);
        private void Info(string msg) => SetEstado(msg, System.Drawing.Color.DodgerBlue);

        // Muestra MessageBox con el error.
        private void Error(string msg)
        {
            SetEstado(msg, System.Drawing.Color.Firebrick);
            MessageBox.Show(this, $"Error: {msg}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetEstado(string msg, System.Drawing.Color color)
        {
            lblEstado.Text = msg;
            lblEstado.ForeColor = color;
        }
        private void MostrarAsignacion()
        {
            var texto = string.Join(Environment.NewLine, new[]
            {
                "Ejercicio 2 – Árbol de Nombres",
                "",
                "• Insertar nombres",
                "• Buscar nombre específico",
                "• Mostrar nombres ordenados (Preorden, Inorden, Postorden)",
                "• Mensaje si el árbol está vacío"
            });

            MessageBox.Show(this, texto, "Asignación del ejercicio", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAsignación_Click(object sender, EventArgs e)
        {
            MostrarAsignacion();
        }

        private void Glosario()
        {
            var texto = string.Join(Environment.NewLine, new[]
            {
                "Intrucciones básicas:",
                "",
                "• Para insertar nombres rellena la textbox",
                "• Para elegir el nodo sobre el cual añadir seleccionalo haciendo click en el",
                "• Selecciona donde lo vas a agregar izquierda, derecha o auto (en el primer espacio disponible)",
                "• Derecha (abajo) | Izquierda (arriba)"
            });
            MessageBox.Show(this, texto, "Glosario", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Glosario();
        }
    }
}