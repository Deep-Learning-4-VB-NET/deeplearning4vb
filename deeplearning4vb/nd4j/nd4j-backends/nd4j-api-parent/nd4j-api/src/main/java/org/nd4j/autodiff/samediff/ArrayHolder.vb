Imports System.Collections.Generic
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

Namespace org.nd4j.autodiff.samediff


	Public Interface ArrayHolder

		''' <returns> True if an array by that name exists </returns>
		Function hasArray(ByVal name As String) As Boolean

		''' <param name="name"> Name of the array to get </param>
		''' <returns> The array, or null if no array with that name exists </returns>
		Function getArray(ByVal name As String) As INDArray

		''' <summary>
		''' Set the array for the specified name (new array, or replace if it already exists)
		''' </summary>
		''' <param name="name">  Name of the array </param>
		''' <param name="array"> Array to set </param>
		Sub setArray(ByVal name As String, ByVal array As INDArray)

		''' <summary>
		''' Remove the array from the ArrayHolder, returning it (if it exists)
		''' </summary>
		''' <param name="name"> Name of the array to return </param>
		''' <returns> The now-removed array </returns>
		Function removeArray(ByVal name As String) As INDArray

		''' <returns> Number of arrays in the ArrayHolder </returns>
		Function size() As Integer

		''' <summary>
		''' Initialize from the specified array holder.
		''' This clears all internal arrays, and adds all arrays from the specified array holder
		''' </summary>
		''' <param name="arrayHolder"> Array holder to initialize this based on </param>
		Sub initFrom(ByVal arrayHolder As ArrayHolder)

		''' <returns> Names of the arrays currently in the ArrayHolder </returns>
		Function arrayNames() As ICollection(Of String)

		''' <summary>
		''' Rename the entry with the specified name
		''' </summary>
		''' <param name="from"> Original name </param>
		''' <param name="to">   New name </param>
		Sub rename(ByVal from As String, ByVal [to] As String)
	End Interface

End Namespace