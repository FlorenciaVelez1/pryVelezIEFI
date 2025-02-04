﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Agrego lo systems que usare en el proyecto
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
//Agrego system para usar el streamwriter
using System.IO;
//Agrego system para el procedimiento Imprimir
using System.Drawing;
using System.Drawing.Printing;
namespace pryVelezFunesIEFI
{
    internal class clsGimnasio
    {
        //Conexion a la base de datos
        private OleDbConnection Conexion = new OleDbConnection();
        private OleDbCommand Comando = new OleDbCommand();
        private OleDbDataAdapter Adaptador = new OleDbDataAdapter();
        //Agrego la ruta
        private string Ruta = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:VelezBaseDatosIEFI.accdb";
        private string Tabla = "Gimnasio";
        //Declaro de manera local las variables ( los campos el Add Socio)
        private Int32 varIDSocio;
        private string varNombreApellido;
        private string varDireccion;
        private Int32 varCodigoBarrio;
        private Int32 varCodigoActividad;
        private Int32 varTelefono;
        //Declaro variables que voy a usar para para contar los clientes, promedio y saldo total.
        public decimal VarTotalIngreso;
        public Int32 VarCantCliente;
        public decimal VarPromedio;
        //Creo VarBandera para verificar
        public bool VarBandera;
        //Creo vector para llenarlo con los datos de saldos
        decimal [] VecSaldosClientes = new decimal [500];
        public Int32 SocioID
        {
            get { return varIDSocio; }
            set { varIDSocio = value; }
        }
        public string Nom_Apellido
        {
            get { return varNombreApellido; }
            set { varNombreApellido = value; }
        }
        public string DireccionSocio
        {
            get { return varDireccion; }
            set { varDireccion = value; }
        }
        public Int32 CodBarrio
        {
            get { return varCodigoBarrio; }
            set { varCodigoBarrio = value; }
        }
        public Int32 CodActividad
        {
            get { return varCodigoActividad; }
            set { varCodigoActividad = value; }
        }
        public Int32 TelefonoSocio
        {
            get { return varTelefono; }
            set { varTelefono = value; }
        }
        public void Agregar()
        {
            try
            {
                //Instruccion sql, creo variable con la intruccion concatenada y utilizo la variable luego
                string Sql = "INSERT INTO Gimnasio ([ID Socio],[Nombre y Apellido], [Direccion], [Codigo Barrio], [Codigo Actividad], [Telefono])" +
                    "VALUES ('" + SocioID + "','" + Nom_Apellido + "','" + DireccionSocio + "','" + CodBarrio + "','" + CodActividad + "','" + TelefonoSocio + "')";
                Conexion.ConnectionString = Ruta;
                Conexion.Open();
                Comando.Connection = Conexion;
                Comando.CommandType = CommandType.Text;
                Comando.CommandText = Sql;
                Comando.ExecuteNonQuery();
                Conexion.Close();
                MessageBox.Show("Se han registrado los datos correctamente.");
            }
            catch (Exception)
            {
                MessageBox.Show("No se han podido registrar los datos.");
            }
        }
        public void Buscar(Int32 IDSocio)
        {
            VarBandera = true;
            try
            {
                Conexion.ConnectionString = Ruta;
                Conexion.Open();
                Comando.Connection = Conexion;
                Comando.CommandType = CommandType.TableDirect;
                Comando.CommandText = Tabla;
                //Recibe el contenido de la tabla
                OleDbDataReader Lector = Comando.ExecuteReader();
                //Si hay filas que leer entra en el "si"
                if (Lector.HasRows)
                {
                    while (Lector.Read())
                    {
                        if (Lector.GetInt32(0) == IDSocio)
                        {
                            varIDSocio = Lector.GetInt32(0);
                            varNombreApellido = Lector.GetString(1);
                            varDireccion = Lector.GetString(2);
                            varCodigoBarrio = Lector.GetInt32(3);
                            varCodigoActividad = Lector.GetInt32(4);
                            varTelefono = Lector.GetInt32(5);
                            VarBandera = false;
                        }
                    }
                }
                Conexion.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Hubo un error al buscar el ID Socio.");
            }
        }
        public void Modificar(Int32 IDSOCIO)
        {
            try
            {
                string Sql = "UPDATE Gimnasio SET [Nombre y Apellido]= '" + Nom_Apellido + "', [Direccion]= '" + DireccionSocio + "', [Codigo Barrio]= " +
                CodBarrio + ", [Codigo Actividad]= " + CodActividad + ", [Telefono]= " + TelefonoSocio + " WHERE [ID Socio] = " + IDSOCIO + "";
                Conexion.ConnectionString = Ruta;
                Conexion.Open();
                Comando.Connection = Conexion;
                Comando.CommandType = CommandType.Text;
                Comando.CommandText = Sql;
                Comando.ExecuteNonQuery();
                Conexion.Close();
                MessageBox.Show("La informacion ha sido modificada exitosamente.");

            }
            catch (Exception)
            {
                MessageBox.Show("No se ha podido modificar la informacion.");
            }
        }
        public void Eliminar(Int32 IDSOCIO)
        {
            try
            {
                string Sql = "DELETE FROM Gimnasio WHERE (" + IDSOCIO + "= [ID Socio])";
                Conexion.ConnectionString = Ruta;
                Conexion.Open();
                Comando.Connection = Conexion;
                Comando.CommandType = CommandType.Text;
                Comando.CommandText = Sql;
                Comando.ExecuteNonQuery();
                Conexion.Close();
                MessageBox.Show("Datos borrados con exito");

            }
            catch (Exception)
            {
                MessageBox.Show("No se ha podido eliminar el cliente completamente");
            }
        }
        public void ListarGrillaAct(DataGridView GrillaActividad, Int32 CodActividad)
        {
            try
            {
                Conexion.ConnectionString = Ruta;
                Conexion.Open();
                Comando.Connection = Conexion;
                Comando.CommandType = CommandType.TableDirect;
                Comando.CommandText = Tabla;
                GrillaActividad.Rows.Clear();
                OleDbDataReader Lector = Comando.ExecuteReader();
                VarCantCliente = 0;
                VarPromedio = 0;
                VarTotalIngreso = 0;
                if (Lector.HasRows)
                {
                    while (Lector.Read())
                    {
                        if (Lector.GetInt32(4) == CodActividad)
                        {
                            //llamo las cls para cambiar los numeros por los nombres correspondientes
                            Int32 codBarrio = Lector.GetInt32(3);
                            Int32 codAct = Lector.GetInt32(4);
                            Int32 IDSOCIO = Lector.GetInt32(0);
                            clsBarrio BarrioConsulta = new clsBarrio();
                            BarrioConsulta.BuscarBarrio(codBarrio);
                            clsActividad ActConsulta = new clsActividad();
                            ActConsulta.BuscarActividad(codAct);
                            clsInscripcion InfoClienteIns = new clsInscripcion();
                            InfoClienteIns.Buscar(IDSOCIO);
                            GrillaActividad.Rows.Add(Lector.GetInt32(0), Lector.GetString(1), Lector.GetString(2), BarrioConsulta.NomBarrio, ActConsulta.NomActividad, Lector.GetInt32(5), InfoClienteIns.Fecha_Inscripcion, InfoClienteIns.SaldoSocio);
                            VarCantCliente++;
                            VarTotalIngreso = VarTotalIngreso + InfoClienteIns.SaldoSocio;
                            VarPromedio = VarTotalIngreso/VarCantCliente;
                        }
                    }
                }
                Conexion.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("No se ha podido cargar la informacion.");
            }
        }
        public void ExportarClientesAct(Int32 idACtividad)
        {
            try
            {
                Conexion.ConnectionString = Ruta;
                Conexion.Open();
                Comando.Connection = Conexion; 
                Comando.CommandType = CommandType.TableDirect;
                Comando.CommandText = Tabla;
                OleDbDataReader Lector = Comando.ExecuteReader();
                //Creo el archivo para exportar los datos
                StreamWriter ExportarDatos = new StreamWriter("ExportarClientesPorActividad.csv", false, Encoding.UTF8);
                ExportarDatos.WriteLine("Listado de Socios");
                ExportarDatos.WriteLine("ID Socio;Nombre y Apellido;Direccion;Barrio; Actividad; Telefono; Fecha de Inscripcion;Saldo");
                VarCantCliente = 0;
                VarTotalIngreso = 0;
                VarPromedio = 0;
                if (Lector.HasRows)
                {
                    while (Lector.Read())
                    {
                        if (idACtividad == Lector.GetInt32(4))
                        {
                            //llamo las cls para cambiar los numeros por los nombres correspondientes
                            Int32 codBarrio = Lector.GetInt32(3);
                            Int32 codAct = Lector.GetInt32(4);
                            Int32 IDSOCIO = Lector.GetInt32(0);
                            clsBarrio BarrioConsulta = new clsBarrio();
                            BarrioConsulta.BuscarBarrio(codBarrio);
                            clsActividad ActConsulta = new clsActividad();
                            ActConsulta.BuscarActividad(codAct);
                            clsInscripcion InfoClienteIns = new clsInscripcion();
                            InfoClienteIns.Buscar(IDSOCIO);
                            ExportarDatos.Write(Lector.GetInt32(0));
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(Lector.GetString(1));
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(Lector.GetString(2));
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(BarrioConsulta.NomBarrio);
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(ActConsulta.NomActividad);
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(Lector.GetInt32(5));
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(InfoClienteIns.Fecha_Inscripcion);
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(InfoClienteIns.SaldoSocio);
                            VarCantCliente++;
                            VarTotalIngreso = VarTotalIngreso + InfoClienteIns.SaldoSocio;
                            VarPromedio = VarTotalIngreso / VarCantCliente;
                            ExportarDatos.Write("\n");
                        }
                    }
                    ExportarDatos.Write("Cantidad de socios:");
                    ExportarDatos.WriteLine(VarCantCliente);
                    ExportarDatos.Write("Total de saldo:");
                    ExportarDatos.WriteLine(VarTotalIngreso);
                    ExportarDatos.Write("Promedio:");
                    ExportarDatos.WriteLine(VarPromedio);
                }
                Conexion.Close();
                ExportarDatos.Close();
                MessageBox.Show("Tus datos han sido exportados con exitosamente.");
            }
            catch (Exception)
            {
                MessageBox.Show("Tus datos no han podido ser exportados.");

            }
        }
        public void ListarGrillaBar(DataGridView GrillaBarrio, Int32 CodBarrio)
        {
            try
            {
                Conexion.ConnectionString = Ruta;
                Conexion.Open();
                Comando.Connection = Conexion;
                Comando.CommandType = CommandType.TableDirect;
                Comando.CommandText = Tabla;
                GrillaBarrio.Rows.Clear();
                OleDbDataReader Lector = Comando.ExecuteReader();
                VarCantCliente = 0;
                VarPromedio = 0;
                VarTotalIngreso = 0;
                if (Lector.HasRows)
                {
                    while (Lector.Read())
                    {
                        if (Lector.GetInt32(3) == CodBarrio)
                        {
                            //llamo las cls para cambiar los numeros por los nombres correspondientes
                            Int32 codBarrio = Lector.GetInt32(3);
                            Int32 codAct = Lector.GetInt32(4);
                            Int32 IDSOCIO = Lector.GetInt32(0);
                            clsBarrio BarrioConsulta = new clsBarrio();
                            BarrioConsulta.BuscarBarrio(codBarrio);
                            clsActividad ActConsulta = new clsActividad();
                            ActConsulta.BuscarActividad(codAct);
                            clsInscripcion InfoClienteIns = new clsInscripcion();
                            InfoClienteIns.Buscar(IDSOCIO);
                            GrillaBarrio.Rows.Add(Lector.GetInt32(0), Lector.GetString(1), Lector.GetString(2), BarrioConsulta.NomBarrio, ActConsulta.NomActividad, Lector.GetInt32(5), InfoClienteIns.Fecha_Inscripcion, InfoClienteIns.SaldoSocio);
                            VarCantCliente++;
                            VarTotalIngreso = VarTotalIngreso + InfoClienteIns.SaldoSocio;
                            VarPromedio = VarTotalIngreso / VarCantCliente;
                        }
                    }
                }
                Conexion.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("No se ha podido cargar la informacion.");
            }
        }
        public void ListarGrillaClie(DataGridView GrillaBarrio)
        {
            try
            {
                Conexion.ConnectionString = Ruta;
                Conexion.Open();
                Comando.Connection = Conexion;
                Comando.CommandType = CommandType.TableDirect;
                Comando.CommandText = Tabla;
                GrillaBarrio.Rows.Clear();
                OleDbDataReader Lector = Comando.ExecuteReader();
                VarCantCliente = 0;
                VarPromedio = 0;
                VarTotalIngreso = 0;
                if (Lector.HasRows)
                {
                    int indiceVector;
                    indiceVector = 0;
                    while (Lector.Read())
                    {
                        //llamo las cls para cambiar los numeros por los nombres correspondientes
                        Int32 codBarrio = Lector.GetInt32(3);
                        Int32 codAct = Lector.GetInt32(4);
                        Int32 IDSOCIO = Lector.GetInt32(0);
                        clsBarrio BarrioConsulta = new clsBarrio();
                        BarrioConsulta.BuscarBarrio(codBarrio);
                        clsActividad ActConsulta = new clsActividad();
                        ActConsulta.BuscarActividad(codAct);
                        clsInscripcion InfoClienteIns = new clsInscripcion();
                        InfoClienteIns.Buscar(IDSOCIO);
                        GrillaBarrio.Rows.Add(Lector.GetInt32(0), Lector.GetString(1), Lector.GetString(2), BarrioConsulta.NomBarrio, ActConsulta.NomActividad, Lector.GetInt32(5), InfoClienteIns.Fecha_Inscripcion, InfoClienteIns.SaldoSocio);
                        VarCantCliente++;
                        VarTotalIngreso = VarTotalIngreso + InfoClienteIns.SaldoSocio;
                        VarPromedio = VarTotalIngreso / VarCantCliente;
                        //Le agrego los datos al vector
                        VecSaldosClientes[indiceVector] = InfoClienteIns.SaldoSocio;
                        indiceVector++;
                    }
                }
                Conexion.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("No se ha podido cargar la informacion.");
            }
        }
        public void ExportarClientesBar(Int32 idBarrio)
        {
            try
            {
                Conexion.ConnectionString = Ruta;
                Conexion.Open();
                Comando.Connection = Conexion;
                Comando.CommandType = CommandType.TableDirect;
                Comando.CommandText = Tabla;
                OleDbDataReader Lector = Comando.ExecuteReader();
                //Creo el archivo para exportar los datos
                StreamWriter ExportarDatos = new StreamWriter("ExportarClientesPorBarrio.csv", false, Encoding.UTF8);
                ExportarDatos.WriteLine("Listado de Socios");
                ExportarDatos.WriteLine("ID Socio;Nombre y Apellido;Direccion;Barrio; Actividad; Telefono; Fecha de Inscripcion;Saldo");
                VarCantCliente = 0;
                VarTotalIngreso = 0;
                VarPromedio = 0;
                if (Lector.HasRows)
                {
                    while (Lector.Read())
                    {
                        if (idBarrio == Lector.GetInt32(3))
                        {
                            //llamo las cls para cambiar los numeros por los nombres correspondientes
                            Int32 codBarrio = Lector.GetInt32(3);
                            Int32 codAct = Lector.GetInt32(4);
                            Int32 IDSOCIO = Lector.GetInt32(0);
                            clsBarrio BarrioConsulta = new clsBarrio();
                            BarrioConsulta.BuscarBarrio(codBarrio);
                            clsActividad ActConsulta = new clsActividad();
                            ActConsulta.BuscarActividad(codAct);
                            clsInscripcion InfoClienteIns = new clsInscripcion();
                            InfoClienteIns.Buscar(IDSOCIO);
                            ExportarDatos.Write(Lector.GetInt32(0));
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(Lector.GetString(1));
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(Lector.GetString(2));
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(BarrioConsulta.NomBarrio);
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(ActConsulta.NomActividad);
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(Lector.GetInt32(5));
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(InfoClienteIns.Fecha_Inscripcion);
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(InfoClienteIns.SaldoSocio);
                            VarCantCliente++;
                            VarTotalIngreso = VarTotalIngreso + InfoClienteIns.SaldoSocio;
                            VarPromedio = VarTotalIngreso / VarCantCliente;
                            ExportarDatos.Write("\n");
                        }
                    }
                    ExportarDatos.Write("Cantidad de socios:");
                    ExportarDatos.WriteLine(VarCantCliente);
                    ExportarDatos.Write("Total de saldo:");
                    ExportarDatos.WriteLine(VarTotalIngreso);
                    ExportarDatos.Write("Promedio:");
                    ExportarDatos.WriteLine(((short)VarPromedio));
                }
                Conexion.Close();
                ExportarDatos.Close();
                MessageBox.Show("Tus datos han sido exportados con exitosamente.");
            }
            catch (Exception)
            {
                MessageBox.Show("Tus datos no han podido ser exportados.");
            }
        }
        public void ExportarClientes()
        {
            try
            {
                Conexion.ConnectionString = Ruta;
                Conexion.Open();
                Comando.Connection = Conexion;
                Comando.CommandType = CommandType.TableDirect;
                Comando.CommandText = Tabla;
                OleDbDataReader Lector = Comando.ExecuteReader();
                //Creo el archivo para exportar los datos
                StreamWriter ExportarDatos = new StreamWriter("ExportarClientes.csv", false, Encoding.UTF8);
                ExportarDatos.WriteLine("Listado de Socios");
                ExportarDatos.WriteLine("ID Socio;Nombre y Apellido;Direccion;Barrio; Actividad; Telefono; Fecha de Inscripcion;Saldo");
                VarCantCliente = 0;
                VarTotalIngreso = 0;
                VarPromedio = 0;
                if (Lector.HasRows)
                {
                    while (Lector.Read())
                    {
                            //llamo las cls para cambiar los numeros por los nombres correspondientes
                            Int32 codBarrio = Lector.GetInt32(3);
                            Int32 codAct = Lector.GetInt32(4);
                            Int32 IDSOCIO = Lector.GetInt32(0);
                            clsBarrio BarrioConsulta = new clsBarrio();
                            BarrioConsulta.BuscarBarrio(codBarrio);
                            clsActividad ActConsulta = new clsActividad();
                            ActConsulta.BuscarActividad(codAct);
                            clsInscripcion InfoClienteIns = new clsInscripcion();
                            InfoClienteIns.Buscar(IDSOCIO);
                            ExportarDatos.Write(Lector.GetInt32(0));
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(Lector.GetString(1));
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(Lector.GetString(2));
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(BarrioConsulta.NomBarrio);
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(ActConsulta.NomActividad);
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(Lector.GetInt32(5));
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(InfoClienteIns.Fecha_Inscripcion);
                            ExportarDatos.Write(";");
                            ExportarDatos.Write(InfoClienteIns.SaldoSocio);
                            VarCantCliente++;
                            VarTotalIngreso = VarTotalIngreso + InfoClienteIns.SaldoSocio;
                            VarPromedio = VarTotalIngreso / VarCantCliente;
                            ExportarDatos.Write("\n");
                    }
                    ExportarDatos.Write("Cantidad de socios:");
                    ExportarDatos.WriteLine(VarCantCliente);
                    ExportarDatos.Write("Total de saldo:");
                    ExportarDatos.WriteLine(VarTotalIngreso);
                    ExportarDatos.Write("Promedio:");
                    ExportarDatos.WriteLine(((short)VarPromedio));
                }
                Conexion.Close();
                ExportarDatos.Close();
                MessageBox.Show("Tus datos han sido exportados con exitosamente.");
            }
            catch (Exception)
            {
                MessageBox.Show("Tus datos no han podido ser exportados.");
            }
        }
        public void ImprimirCli(PrintPageEventArgs reporte)
        {
            try
            {
                Font TipoLetra = new Font("Arial", 8);
                Font TipoLTitulo = new Font("Arial", 10);
                Font TipoLTituloPrin = new Font("Arial", 20);
                Int32 LineaPag = 150;
                reporte.Graphics.DrawString("Listado de Clientes", TipoLTituloPrin, Brushes.Black, 280, 50);
                reporte.Graphics.DrawString("ID Socio", TipoLTitulo, Brushes.DarkBlue, 30, 125);
                reporte.Graphics.DrawString("Nombre y Apellido", TipoLTitulo, Brushes.DarkBlue, 100, 125);
                reporte.Graphics.DrawString("Dirección", TipoLTitulo, Brushes.DarkBlue, 240, 125);
                reporte.Graphics.DrawString("Barrio", TipoLTitulo, Brushes.DarkBlue, 390, 125);
                reporte.Graphics.DrawString("Actividad", TipoLTitulo, Brushes.DarkBlue, 525, 125);
                reporte.Graphics.DrawString("Teléfono", TipoLTitulo, Brushes.DarkBlue, 625, 125);
                reporte.Graphics.DrawString("Inscripción", TipoLTitulo, Brushes.DarkBlue, 690, 125);
                reporte.Graphics.DrawString("Saldo", TipoLTitulo, Brushes.DarkBlue, 765, 125);
                Conexion.ConnectionString = Ruta;
                Conexion.Open();
                Comando.Connection = Conexion;
                Comando.CommandType = CommandType.TableDirect;
                Comando.CommandText = Tabla;
                OleDbDataReader Lector = Comando.ExecuteReader();
                if (Lector.HasRows)
                {
                    while (Lector.Read())
                    {
                        Int32 codBarrio = Lector.GetInt32(3);
                        Int32 codAct = Lector.GetInt32(4);
                        Int32 IDSOCIO = Lector.GetInt32(0);
                        clsBarrio BarrioConsulta = new clsBarrio();
                        BarrioConsulta.BuscarBarrio(codBarrio);
                        clsActividad ActConsulta = new clsActividad();
                        ActConsulta.BuscarActividad(codAct);
                        clsInscripcion InfoClienteIns = new clsInscripcion();
                        InfoClienteIns.Buscar(IDSOCIO);
                        reporte.Graphics.DrawString(Lector.GetInt32(0).ToString(), TipoLetra, Brushes.Black, 30, LineaPag);
                        reporte.Graphics.DrawString(Lector.GetString(1), TipoLetra, Brushes.Black, 100, LineaPag); 
                        reporte.Graphics.DrawString(Lector.GetString(2), TipoLetra, Brushes.Black, 240, LineaPag); 
                        reporte.Graphics.DrawString(BarrioConsulta.NomBarrio.ToString(), TipoLetra, Brushes.Black, 390, LineaPag); 
                        reporte.Graphics.DrawString(ActConsulta.NomActividad.ToString(), TipoLetra, Brushes.Black, 525, LineaPag);
                        reporte.Graphics.DrawString(Lector.GetInt32(5).ToString(), TipoLetra, Brushes.Black, 625, LineaPag);
                        reporte.Graphics.DrawString(InfoClienteIns.Fecha_Inscripcion.ToString(), TipoLetra, Brushes.Black, 690, LineaPag);
                        reporte.Graphics.DrawString(InfoClienteIns.SaldoSocio.ToString(), TipoLetra, Brushes.Black, 765 , LineaPag);
                        LineaPag = LineaPag + 15;
                    }
                }
                Conexion.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public void ImprimirBar(PrintPageEventArgs reporte, Int32 CodBarrio)
        {
            try
            {
                Font TipoLetra = new Font("Arial", 8);
                Font TipoLTitulo = new Font("Arial", 10);
                Font TipoLTituloPrin = new Font("Arial", 20);
                Int32 LineaPag = 150;
                reporte.Graphics.DrawString("Listado de Clientes", TipoLTituloPrin, Brushes.Black, 280, 50);
                reporte.Graphics.DrawString("ID Socio", TipoLTitulo, Brushes.DarkBlue, 30, 125);
                reporte.Graphics.DrawString("Nombre y Apellido", TipoLTitulo, Brushes.DarkBlue, 100, 125);
                reporte.Graphics.DrawString("Dirección", TipoLTitulo, Brushes.DarkBlue, 240, 125);
                reporte.Graphics.DrawString("Barrio", TipoLTitulo, Brushes.DarkBlue, 390, 125);
                reporte.Graphics.DrawString("Actividad", TipoLTitulo, Brushes.DarkBlue, 525, 125);
                reporte.Graphics.DrawString("Teléfono", TipoLTitulo, Brushes.DarkBlue, 625, 125);
                reporte.Graphics.DrawString("Inscripción", TipoLTitulo, Brushes.DarkBlue, 690, 125);
                reporte.Graphics.DrawString("Saldo", TipoLTitulo, Brushes.DarkBlue, 765, 125);
                Conexion.ConnectionString = Ruta;
                Conexion.Open();
                Comando.Connection = Conexion;
                Comando.CommandType = CommandType.TableDirect;
                Comando.CommandText = Tabla;
                OleDbDataReader Lector = Comando.ExecuteReader();
                if (Lector.HasRows)
                {
                    while (Lector.Read())
                    {
                        if (Lector.GetInt32(3) == CodBarrio)
                        {
                            Int32 codBarrio = Lector.GetInt32(3);
                            Int32 codAct = Lector.GetInt32(4);
                            Int32 IDSOCIO = Lector.GetInt32(0);
                            clsBarrio BarrioConsulta = new clsBarrio();
                            BarrioConsulta.BuscarBarrio(codBarrio);
                            clsActividad ActConsulta = new clsActividad();
                            ActConsulta.BuscarActividad(codAct);
                            clsInscripcion InfoClienteIns = new clsInscripcion();
                            InfoClienteIns.Buscar(IDSOCIO);
                            reporte.Graphics.DrawString(Lector.GetInt32(0).ToString(), TipoLetra, Brushes.Black, 30, LineaPag);
                            reporte.Graphics.DrawString(Lector.GetString(1), TipoLetra, Brushes.Black, 100, LineaPag);
                            reporte.Graphics.DrawString(Lector.GetString(2), TipoLetra, Brushes.Black, 240, LineaPag);
                            reporte.Graphics.DrawString(BarrioConsulta.NomBarrio.ToString(), TipoLetra, Brushes.Black, 390, LineaPag);
                            reporte.Graphics.DrawString(ActConsulta.NomActividad.ToString(), TipoLetra, Brushes.Black, 525, LineaPag);
                            reporte.Graphics.DrawString(Lector.GetInt32(5).ToString(), TipoLetra, Brushes.Black, 625, LineaPag);
                            reporte.Graphics.DrawString(InfoClienteIns.Fecha_Inscripcion.ToString(), TipoLetra, Brushes.Black, 690, LineaPag);
                            reporte.Graphics.DrawString(InfoClienteIns.SaldoSocio.ToString(), TipoLetra, Brushes.Black, 765, LineaPag);
                            LineaPag = LineaPag + 15;
                        }
                    }
                }
                Conexion.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public void ImprimirAct(PrintPageEventArgs reporte,Int32 CodActividad)
        {
            try
            {
                Font TipoLetra = new Font("Arial", 8);
                Font TipoLTitulo = new Font("Arial", 10);
                Font TipoLTituloPrin = new Font("Arial", 20);
                Int32 LineaPag = 150;
                reporte.Graphics.DrawString("Listado de Clientes", TipoLTituloPrin, Brushes.Black, 280, 50);
                reporte.Graphics.DrawString("ID Socio", TipoLTitulo, Brushes.DarkBlue, 30, 125);
                reporte.Graphics.DrawString("Nombre y Apellido", TipoLTitulo, Brushes.DarkBlue, 100, 125);
                reporte.Graphics.DrawString("Dirección", TipoLTitulo, Brushes.DarkBlue, 240, 125);
                reporte.Graphics.DrawString("Barrio", TipoLTitulo, Brushes.DarkBlue, 390, 125);
                reporte.Graphics.DrawString("Actividad", TipoLTitulo, Brushes.DarkBlue, 525, 125);
                reporte.Graphics.DrawString("Teléfono", TipoLTitulo, Brushes.DarkBlue, 625, 125);
                reporte.Graphics.DrawString("Inscripción", TipoLTitulo, Brushes.DarkBlue, 690, 125);
                reporte.Graphics.DrawString("Saldo", TipoLTitulo, Brushes.DarkBlue, 765, 125);
                Conexion.ConnectionString = Ruta;
                Conexion.Open();
                Comando.Connection = Conexion;
                Comando.CommandType = CommandType.TableDirect;
                Comando.CommandText = Tabla;
                OleDbDataReader Lector = Comando.ExecuteReader();
                if (Lector.HasRows)
                {
                    while (Lector.Read())
                    {
                        if (Lector.GetInt32(4) == CodActividad)
                        {
                            Int32 codBarrio = Lector.GetInt32(3);
                            Int32 codAct = Lector.GetInt32(4);
                            Int32 IDSOCIO = Lector.GetInt32(0);
                            clsBarrio BarrioConsulta = new clsBarrio();
                            BarrioConsulta.BuscarBarrio(codBarrio);
                            clsActividad ActConsulta = new clsActividad();
                            ActConsulta.BuscarActividad(codAct);
                            clsInscripcion InfoClienteIns = new clsInscripcion();
                            InfoClienteIns.Buscar(IDSOCIO);
                            reporte.Graphics.DrawString(Lector.GetInt32(0).ToString(), TipoLetra, Brushes.Black, 30, LineaPag);
                            reporte.Graphics.DrawString(Lector.GetString(1), TipoLetra, Brushes.Black, 100, LineaPag);
                            reporte.Graphics.DrawString(Lector.GetString(2), TipoLetra, Brushes.Black, 240, LineaPag);
                            reporte.Graphics.DrawString(BarrioConsulta.NomBarrio.ToString(), TipoLetra, Brushes.Black, 390, LineaPag);
                            reporte.Graphics.DrawString(ActConsulta.NomActividad.ToString(), TipoLetra, Brushes.Black, 525, LineaPag);
                            reporte.Graphics.DrawString(Lector.GetInt32(5).ToString(), TipoLetra, Brushes.Black, 625, LineaPag);
                            reporte.Graphics.DrawString(InfoClienteIns.Fecha_Inscripcion.ToString(), TipoLetra, Brushes.Black, 690, LineaPag);
                            reporte.Graphics.DrawString(InfoClienteIns.SaldoSocio.ToString(), TipoLetra, Brushes.Black, 765, LineaPag);
                            LineaPag = LineaPag + 15;
                        }
                    }
                }
                Conexion.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
