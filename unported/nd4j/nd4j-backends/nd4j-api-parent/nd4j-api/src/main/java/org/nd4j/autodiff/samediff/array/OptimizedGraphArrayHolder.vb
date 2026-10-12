Imports System.Collections.Generic
Imports ArrayHolder = org.nd4j.autodiff.samediff.ArrayHolder
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.common.function
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.nd4j.autodiff.samediff.array


	Public Class OptimizedGraphArrayHolder
		Implements ArrayHolder

		Private ReadOnly underlyingHolder As ArrayHolder
		Private ReadOnly functions As IDictionary(Of String, Supplier(Of INDArray))

		Public Sub New(ByVal underlyingHolder As ArrayHolder)
			Me.underlyingHolder = underlyingHolder
			Me.functions = New Dictionary(Of String, Supplier(Of INDArray))()
		End Sub

		Public Overridable Sub setFunction(ByVal name As String, ByVal fn As Supplier(Of INDArray))
			If underlyingHolder.hasArray(name) Then
				underlyingHolder.removeArray(name)
			End If
			functions(name) = fn
		End Sub

		Public Overridable Function hasArray(ByVal name As String) As Boolean Implements ArrayHolder.hasArray
			Return functions.ContainsKey(name) OrElse underlyingHolder.hasArray(name)
		End Function

		Public Overridable Function getArray(ByVal name As String) As INDArray Implements ArrayHolder.getArray
			If functions.ContainsKey(name) Then
				Return functions(name).get()
			End If
			Return underlyingHolder.getArray(name)
		End Function

		Public Overridable Sub setArray(ByVal name As String, ByVal array As INDArray) Implements ArrayHolder.setArray
			Preconditions.checkState(Not functions.ContainsKey(name), "Cannot set array when existing array is only accessible via a function")
			underlyingHolder.setArray(name, array)
		End Sub

		Public Overridable Function removeArray(ByVal name As String) As INDArray Implements ArrayHolder.removeArray
			Dim s As Supplier(Of INDArray) = functions.Remove(name)
			If s IsNot Nothing Then
				Return s.get()
			End If
			Return underlyingHolder.removeArray(name)
		End Function

		Public Overridable Function size() As Integer Implements ArrayHolder.size
			Return underlyingHolder.size() + functions.Count
		End Function

		Public Overridable Sub initFrom(ByVal arrayHolder As ArrayHolder)
			underlyingHolder.initFrom(arrayHolder)
		End Sub

		Public Overridable Function arrayNames() As ICollection(Of String)
			Dim set As ISet(Of String) = New HashSet(Of String)()
			set.addAll(underlyingHolder.arrayNames())
			set.addAll(functions.Keys)
			Return set
		End Function

		Public Overridable Sub rename(ByVal from As String, ByVal [to] As String) Implements ArrayHolder.rename
			If functions.ContainsKey(from) Then
				functions([to]) = functions.Remove(from)
			Else
				underlyingHolder.rename(from, [to])
			End If
		End Sub
	End Class

End Namespace