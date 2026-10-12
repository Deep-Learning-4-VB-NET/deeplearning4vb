Imports System
Imports System.Collections.Generic
Imports ResultSetIterator = org.apache.commons.dbutils.ResultSetIterator

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

Namespace org.datavec.jdbc.util

	Public Class ResettableResultSetIterator
		Implements IEnumerator(Of Object())

		Private rs As ResultSet
		Private base As ResultSetIterator

		Public Sub New(ByVal rs As ResultSet)
			Me.rs = rs
			Me.base = New ResultSetIterator(rs)
		End Sub

		Public Overridable Sub reset()
			Try
				Me.rs.beforeFirst()
			Catch e As SQLException
				Throw New Exception("Could not reset ResultSetIterator", e)
			End Try
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return Me.base.hasNext()
		End Function

		Public Overrides Function [next]() As Object()
			Return base.next()
		End Function

		Public Overrides Sub remove()
			base.remove()
		End Sub
	End Class

End Namespace