Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports [Function] = org.apache.spark.api.java.function.Function
Imports IAssociativeReducer = org.datavec.api.transform.reduce.IAssociativeReducer
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.spark.transform.reduce


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class ReducerFunction implements org.apache.spark.api.java.function.@Function<Iterable<java.util.List<org.datavec.api.writable.Writable>>, java.util.List<org.datavec.api.writable.Writable>>
	Public Class ReducerFunction
		Implements [Function](Of IEnumerable(Of IList(Of Writable)), IList(Of Writable))

		Private ReadOnly reducer As IAssociativeReducer

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> call(Iterable<java.util.List<org.datavec.api.writable.Writable>> lists) throws Exception
		Public Overrides Function [call](ByVal lists As IEnumerable(Of IList(Of Writable))) As IList(Of Writable)

			For Each c As IList(Of Writable) In lists
				reducer.aggregableReducer().accept(c)
			Next c

			Return reducer.aggregableReducer().get()
		End Function
	End Class

End Namespace