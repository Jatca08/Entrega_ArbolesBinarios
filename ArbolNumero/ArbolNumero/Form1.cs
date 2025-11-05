using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

                                                                                                                            namespace ArbolNumero
{
    // Formulario principal para gestionar y visualizar un árbol binario de números
    public partial class Form1 : Form
    {
        // Controles reutilizables como campos para acceder desde eventos
        private TextBox inputTextBox;      // Caja de texto para ingresar números
        private ListBox lstRealTime;       // Muestra los valores insertados en orden real
        private ListBox lstResultado;      // Muestra el resultado del recorrido
        private ComboBox cmbRecorrido;     // Selección del tipo de recorrido
        private ComboBox cmbInsercion;     // Selección del modo de inserción
        private Button btnInsertar;        // Botón para insertar un número
        private Button btnRecorrer;        // Botón para mostrar recorrido
        private Button btnBuscar;          // Botón para buscar un número
        private Button btnReset;           // Botón para resetear el árbol
        private TreeView treeView;         // Visualización gráfica del árbol
        private ArbolBinario arbol;        // Instancia del árbol binario

        public Form1()
        {
            InitializeComponent();
            arbol = new ArbolBinario();
            BuildUI(); // Construye la interfaz de usuario
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Evento de carga del formulario (no utilizado)
        }

        // Construye y configura todos los controles de la interfaz
        private void BuildUI()
        {
            this.Text = "Árbol Binario de Números";
            this.Size = new Size(760, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Etiqueta de instrucción
            Label lblInstruccion = new Label();
            lblInstruccion.Text = "Ingrese un número (entero):";
            lblInstruccion.Location = new Point(10, 12);
            lblInstruccion.Size = new Size(220, 20);
            this.Controls.Add(lblInstruccion);

            // Caja de texto para entrada de números
            inputTextBox = new TextBox();
            inputTextBox.Location = new Point(10, 36);
            inputTextBox.Size = new Size(220, 24);
            this.Controls.Add(inputTextBox);

            // Botón para insertar número
            btnInsertar = new Button();
            btnInsertar.Text = "Insertar";
            btnInsertar.Location = new Point(240, 34);
            btnInsertar.Click += BtnInsertar_Click;
            this.Controls.Add(btnInsertar);

            // Botón para buscar número
            btnBuscar = new Button();
            btnBuscar.Text = "Buscar";
            btnBuscar.Location = new Point(320, 34);
            btnBuscar.Click += BtnBuscar_Click;
            this.Controls.Add(btnBuscar);

            // Botón para resetear el árbol
            btnReset = new Button();
            btnReset.Text = "Resetear Árbol";
            btnReset.Location = new Point(400, 34);
            btnReset.Click += BtnReset_Click;
            this.Controls.Add(btnReset);

            // Etiqueta para modo de inserción
            Label lblModo = new Label();
            lblModo.Text = "Modo inserción:";
            lblModo.Location = new Point(10, 70);
            lblModo.Size = new Size(120, 18);
            this.Controls.Add(lblModo);

            // ComboBox para seleccionar modo de inserción
            cmbInsercion = new ComboBox();
            cmbInsercion.Location = new Point(10, 92);
            cmbInsercion.Size = new Size(220, 24);
            cmbInsercion.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbInsercion.Items.AddRange(new string[] { "Predeterminado (BST)", "Forzar Izquierda", "Forzar Derecha" });
            cmbInsercion.SelectedIndex = 0;
            this.Controls.Add(cmbInsercion);

            // Etiqueta para tipo de recorrido
            Label lblRecorrido = new Label();
            lblRecorrido.Text = "Recorrido para mostrar:";
            lblRecorrido.Location = new Point(250, 70);
            lblRecorrido.Size = new Size(160, 18);
            this.Controls.Add(lblRecorrido);

            // ComboBox para seleccionar tipo de recorrido
            cmbRecorrido = new ComboBox();
            cmbRecorrido.Location = new Point(250, 92);
            cmbRecorrido.Size = new Size(190, 24);
            cmbRecorrido.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRecorrido.Items.AddRange(new string[] { "Inorden", "Preorden", "Postorden" });
            cmbRecorrido.SelectedIndex = 0;
            this.Controls.Add(cmbRecorrido);

            // Botón para mostrar recorrido
            btnRecorrer = new Button();
            btnRecorrer.Text = "Mostrar Recorrido";
            btnRecorrer.Location = new Point(460, 92);
            btnRecorrer.Click += BtnRecorrer_Click;
            this.Controls.Add(btnRecorrer);

            // Etiqueta y ListBox para mostrar valores insertados
            Label lblRealTime = new Label();
            lblRealTime.Text = "Entrada (orden real):";
            lblRealTime.Location = new Point(10, 130);
            lblRealTime.Size = new Size(200, 18);
            this.Controls.Add(lblRealTime);

            lstRealTime = new ListBox();
            lstRealTime.Location = new Point(10, 152);
            lstRealTime.Size = new Size(220, 300);
            this.Controls.Add(lstRealTime);

            // Etiqueta y ListBox para mostrar resultado del recorrido
            Label lblResultado = new Label();
            lblResultado.Text = "Resultado del recorrido:";
            lblResultado.Location = new Point(250, 130);
            lblResultado.Size = new Size(200, 18);
            this.Controls.Add(lblResultado);

            lstResultado = new ListBox();
            lstResultado.Location = new Point(250, 152);
            lstResultado.Size = new Size(220, 300);
            this.Controls.Add(lstResultado);

            // Etiqueta y TreeView para visualizar el árbol
            Label lblTree = new Label();
            lblTree.Text = "Visualización del árbol (TreeView):";
            lblTree.Location = new Point(490, 130);
            lblTree.Size = new Size(220, 18);
            this.Controls.Add(lblTree);

            treeView = new TreeView();
            treeView.Location = new Point(490, 152);
            treeView.Size = new Size(250, 300);
            treeView.HideSelection = false;
            this.Controls.Add(treeView);

            // Inicializar vista vacía del árbol
            ActualizarTreeView();
        }

        // Evento al hacer clic en "Insertar"
        private void BtnInsertar_Click(object sender, EventArgs e)
        {
            string text = inputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show("Por favor ingresa un número.", "Entrada vacía", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                inputTextBox.Focus();
                return;
            }

            int valor;
            try
            {
                // Validar que la entrada sea un número entero
                if (!int.TryParse(text, out valor))
                {
                    MessageBox.Show("Entrada inválida. Debe ser un número entero.", "Error de formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    inputTextBox.SelectAll();
                    inputTextBox.Focus();
                    return;
                }

                // Validar que no se repita el valor
                if (arbol.Contiene(valor))
                {
                    MessageBox.Show("Valor repetido en el árbol. Introduce otro valor.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    inputTextBox.SelectAll();
                    inputTextBox.Focus();
                    return;
                }

                // Seleccionar modo de inserción según ComboBox
                ArbolBinario.ModoInsercion modo = ArbolBinario.ModoInsercion.Predeterminado;
                switch (cmbInsercion.SelectedIndex)
                {
                    case 1:
                        modo = ArbolBinario.ModoInsercion.Izquierda;
                        break;
                    case 2:
                        modo = ArbolBinario.ModoInsercion.Derecha;
                        break;
                    default:
                        modo = ArbolBinario.ModoInsercion.Predeterminado;
                        break;
                }

                // Insertar el valor en el árbol
                arbol.Insertar(valor, modo);

                // Actualizar la lista y la visualización del árbol
                lstRealTime.Items.Add(valor);
                ActualizarTreeView();

                inputTextBox.Clear();
                inputTextBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al insertar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento al hacer clic en "Mostrar Recorrido"
        private void BtnRecorrer_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar si el árbol está vacío
                if (arbol.EstaVacio())
                {
                    MessageBox.Show("El árbol está vacío.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Seleccionar método de recorrido según ComboBox
                ArbolBinario.MetodoRecorrido metodo = ArbolBinario.MetodoRecorrido.InOrden;
                switch (cmbRecorrido.SelectedIndex)
                {
                    case 0:
                        metodo = ArbolBinario.MetodoRecorrido.InOrden;
                        break;
                    case 1:
                        metodo = ArbolBinario.MetodoRecorrido.PreOrden;
                        break;
                    case 2:
                        metodo = ArbolBinario.MetodoRecorrido.PostOrden;
                        break;
                }

                // Obtener y mostrar el recorrido
                List<int> resultado = arbol.ObtenerRecorrido(metodo);
                lstResultado.Items.Clear();
                foreach (int v in resultado)
                    lstResultado.Items.Add(v);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener recorrido: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento al hacer clic en "Buscar"
        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string text = inputTextBox.Text.Trim();
            int valor;
            // Validar entrada
            if (!int.TryParse(text, out valor))
            {
                MessageBox.Show("Introduce un número entero para buscar.", "Entrada inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                inputTextBox.Focus();
                return;
            }

            try
            {
                // Buscar el valor en el árbol
                bool existe = arbol.Contiene(valor);
                if (existe)
                    MessageBox.Show("El valor existe en el árbol.", "Resultado de búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("El valor NO se encontró en el árbol.", "Resultado de búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la búsqueda: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento al hacer clic en "Resetear Árbol"
        private void BtnReset_Click(object sender, EventArgs e)
        {
            // Confirmar antes de borrar todo
            if (MessageBox.Show("¿Deseas resetear el árbol y borrar entradas?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                arbol = new ArbolBinario();
                lstRealTime.Items.Clear();
                lstResultado.Items.Clear();
                inputTextBox.Clear();
                inputTextBox.Focus();
                ActualizarTreeView();
            }
        }

        // Actualiza el TreeView a partir del nodo raíz del árbol
        private void ActualizarTreeView()
        {
            treeView.BeginUpdate();
            try
            {
                treeView.Nodes.Clear();
                // Si el árbol está vacío, mostrar mensaje
                if (arbol == null || arbol.EstaVacio())
                {
                    treeView.Nodes.Add(new TreeNode("(árbol vacío)"));
                }
                else
                {
                    // Crear nodos visuales recursivamente
                    TreeNode root = CrearTreeNode(arbol.raiz);
                    treeView.Nodes.Add(root);
                    treeView.ExpandAll();
                }
            }
            finally
            {
                treeView.EndUpdate();
            }
        }

        // Crea un TreeNode recursivamente desde un Nodo del árbol
        private TreeNode CrearTreeNode(Nodo nodo)
        {
            if (nodo == null) return new TreeNode("(null)");
            var tnode = new TreeNode(nodo.valor.ToString());
            // Agregar subnodo izquierdo si existe
            if (nodo.izquierdo != null)
            {
                TreeNode left = CrearTreeNode(nodo.izquierdo);
                left.Text = "L: " + left.Text;
                tnode.Nodes.Add(left);
            }
            // Agregar subnodo derecho si existe
            if (nodo.derecho != null)
            {
                TreeNode right = CrearTreeNode(nodo.derecho);
                right.Text = "R: " + right.Text;
                tnode.Nodes.Add(right);
            }
            return tnode;
        }
    }

    // Clase Nodo para el árbol binario
    public class Nodo
    {
        public int valor;         // Valor almacenado en el nodo
        public Nodo izquierdo;    // Referencia al hijo izquierdo
        public Nodo derecho;      // Referencia al hijo derecho

        public Nodo(int valor)
        {
            this.valor = valor;
            izquierdo = null;
            derecho = null;
        }
    }

    // Clase Árbol Binario con soporte para modos de inserción y recorridos
    public class ArbolBinario
    {
        public Nodo raiz; // Nodo raíz del árbol

        public ArbolBinario()
        {
            raiz = null;
        }

        // Tipos de recorrido disponibles
        public enum MetodoRecorrido
        {
            InOrden,
            PreOrden,
            PostOrden
        }

        // Modos de inserción disponibles
        public enum ModoInsercion
        {
            Predeterminado, // BST estándar
            Izquierda,      // Forzar inserción en el subárbol más a la izquierda
            Derecha         // Forzar inserción en el subárbol más a la derecha
        }

        // Verifica si el árbol está vacío
        public bool EstaVacio()
        {
            return raiz == null;
        }

        // Verifica si un valor existe en el árbol
        public bool Contiene(int valor)
        {
            return BuscarRec(raiz, valor);
        }

        // Búsqueda recursiva de un valor
        private bool BuscarRec(Nodo nodo, int valor)
        {
            if (nodo == null) return false;
            if (valor == nodo.valor) return true;
            if (valor < nodo.valor) return BuscarRec(nodo.izquierdo, valor);
            return BuscarRec(nodo.derecho, valor);
        }

        // Inserta un valor en el árbol según el modo seleccionado
        public void Insertar(int valor, ModoInsercion modo)
        {
            if (modo == ModoInsercion.Predeterminado)
            {
                raiz = InsertarRec(raiz, valor);
            }
            else if (modo == ModoInsercion.Izquierda)
            {
                // Inserta siempre en el extremo izquierdo
                if (raiz == null)
                {
                    raiz = new Nodo(valor);
                    return;
                }
                Nodo actual = raiz;
                while (actual.izquierdo != null)
                    actual = actual.izquierdo;
                actual.izquierdo = new Nodo(valor);
            }
            else // Derecha
            {
                // Inserta siempre en el extremo derecho
                if (raiz == null)
                {
                    raiz = new Nodo(valor);
                    return;
                }
                Nodo actual = raiz;
                while (actual.derecho != null)
                    actual = actual.derecho;
                actual.derecho = new Nodo(valor);
            }
        }

        // Inserción estándar tipo BST (usada internamente)
        private Nodo InsertarRec(Nodo nodo, int valor)
        {
            if (nodo == null)
            {
                nodo = new Nodo(valor);
                return nodo;
            }
            if (valor < nodo.valor)
                nodo.izquierdo = InsertarRec(nodo.izquierdo, valor);
            else if (valor > nodo.valor)
                nodo.derecho = InsertarRec(nodo.derecho, valor);
            // Si es igual, no se hace nada (control de duplicados se realiza antes)
            return nodo;
        }

        // Obtiene el recorrido del árbol según el método seleccionado
        public List<int> ObtenerRecorrido(MetodoRecorrido metodo)
        {
            var resultado = new List<int>();
            switch (metodo)
            {
                case MetodoRecorrido.InOrden:
                    RecorrerInOrden(raiz, resultado);
                    break;
                case MetodoRecorrido.PreOrden:
                    RecorrerPreOrden(raiz, resultado);
                    break;
                case MetodoRecorrido.PostOrden:
                    RecorrerPostOrden(raiz, resultado);
                    break;
            }
            return resultado;
        }

        // Recorrido InOrden (izquierda, raíz, derecha)
        private void RecorrerInOrden(Nodo nodo, List<int> res)
        {
            if (nodo == null) return;
            RecorrerInOrden(nodo.izquierdo, res);
            res.Add(nodo.valor);
            RecorrerInOrden(nodo.derecho, res);
        }

        // Recorrido PreOrden (raíz, izquierda, derecha)
        private void RecorrerPreOrden(Nodo nodo, List<int> res)
        {
            if (nodo == null) return;
            res.Add(nodo.valor);
            RecorrerPreOrden(nodo.izquierdo, res);
            RecorrerPreOrden(nodo.derecho, res);
        }

        // Recorrido PostOrden (izquierda, derecha, raíz)
        private void RecorrerPostOrden(Nodo nodo, List<int> res)
        {
            if (nodo == null) return;
            RecorrerPostOrden(nodo.izquierdo, res);
            RecorrerPostOrden(nodo.derecho, res);
            res.Add(nodo.valor);
        }
    }
}