Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports PairFunction = org.apache.spark.api.java.function.PairFunction
Imports Writable = org.datavec.api.writable.Writable
Imports Tuple2 = scala.Tuple2

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

Namespace org.datavec.spark.transform.misc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class ColumnAsKeyPairFunction implements org.apache.spark.api.java.function.PairFunction<java.util.List<org.datavec.api.writable.Writable>, org.datavec.api.writable.Writable, java.util.List<org.datavec.api.writable.Writable>>
	Public Class ColumnAsKeyPairFunction
		Implements PairFunction(Of IList(Of Writable), Writable, IList(Of Writable))

		Private ReadOnly columnIdx As Integer

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public scala.Tuple2<org.datavec.api.writable.Writable, java.util.List<org.datavec.api.writable.Writable>> call(java.util.List<org.datavec.api.writable.Writable> writables) throws Exception
		Public Overrides Function [call](ByVal writables As IList(Of Writable)) As Tuple2(Of Writable, IList(Of Writable))
			Return New Tuple2(Of Writable, IList(Of Writable))(writables(columnIdx), writables)
		End Function
	End Class

End Namespace