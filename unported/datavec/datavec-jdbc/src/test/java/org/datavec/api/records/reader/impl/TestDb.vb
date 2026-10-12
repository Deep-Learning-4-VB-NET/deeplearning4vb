Imports System

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.datavec.api.records.reader.impl


	Public Class TestDb

		Public Shared Sub dropTables(ByVal conn As Connection)
			Try
				Dim stmt As Statement = conn.createStatement()

				Try
					stmt.execute("DROP TABLE Coffee")
					stmt.execute("DROP TABLE AllTypes")
				Catch ex As SQLException
				End Try
			Catch ex As SQLException
				Console.WriteLine("ERROR: " & ex.Message)
				Console.WriteLine(ex.ToString())
				Console.Write(ex.StackTrace)
			End Try
		End Sub

		''' <summary>
		''' The buildCoffeeTable method creates the Coffee table and adds some rows to it.
		''' </summary>
		Public Shared Sub buildCoffeeTable(ByVal conn As Connection)
			Try
				' Get a Statement object.
				Dim stmt As Statement = conn.createStatement()

				' Create the table.
				stmt.execute("CREATE TABLE Coffee (" & "Description CHAR(25), " & "ProdNum CHAR(10) NOT NULL PRIMARY KEY, " & "Price DOUBLE " & ")")

				' Insert row #1.
				stmt.execute("INSERT INTO Coffee VALUES ( " & "'Bolivian Dark', " & "'14-001', " & "8.95 )")

				' Insert row #1.
				stmt.execute("INSERT INTO Coffee VALUES ( " & "'Bolivian Medium', " & "'14-002', " & "8.95 )")

			Catch ex As SQLException
				Console.WriteLine("ERROR: " & ex.Message)
				Console.WriteLine(ex.ToString())
				Console.Write(ex.StackTrace)
			End Try
		End Sub

		Public Shared Sub buildAllTypesTable(ByVal conn As Connection)
			Try
				Dim stmt As Statement = conn.createStatement()

				stmt.execute("CREATE TABLE AllTypes " & "(" & "    boolCol BOOLEAN," & "    dateCol DATE," & "    timeCol TIME," & "    timeStampCol TIMESTAMP," & "    charCol CHAR," & "    longVarCharCol LONG VARCHAR," & "    varCharCol VARCHAR(50)," & "    floatCol FLOAT," & "    realCol REAL," & "    decimalCol DECIMAL," & "    numericCol NUMERIC," & "    doubleCol DOUBLE," & "    integerCol INTEGER," & "    smallIntCol SMALLINT," & "    bitIntCol BIGINT" & ")")

				stmt.execute("INSERT INTO AllTypes (" & "BOOLCOL, DATECOL, TIMECOL, TIMESTAMPCOL, " & "CHARCOL, LONGVARCHARCOL, VARCHARCOL, " & "FLOATCOL, REALCOL, DECIMALCOL, NUMERICCOL, DOUBLECOL, " & "INTEGERCOL, SMALLINTCOL, BITINTCOL) " & "VALUES (TRUE, '2017-08-08', '12:21:05', '2017-08-08 14:24:22.899000000', " & "'A', 'TEST LONGVARCHAR', 'TEST VARCHAR', " & "0.55, 0.5555556, 0, 0, 0.5555555555555556, " & "555555555, 5, 555555555555555555)")
			Catch ex As SQLException
				Console.WriteLine("ERROR: " & ex.Message)
			End Try
		End Sub


	End Class

End Namespace