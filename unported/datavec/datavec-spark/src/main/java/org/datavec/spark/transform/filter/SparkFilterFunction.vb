Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Filter = org.datavec.api.transform.filter.Filter
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

Namespace org.datavec.spark.transform.filter


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class SparkFilterFunction implements org.apache.spark.api.java.function.@Function<java.util.List<org.datavec.api.writable.Writable>, Boolean>
	Public Class SparkFilterFunction
		Implements [Function](Of IList(Of Writable), Boolean)

		Private ReadOnly filter As Filter

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public System.Nullable<Boolean> call(java.util.List<org.datavec.api.writable.Writable> v1) throws Exception
		Public Overrides Function [call](ByVal v1 As IList(Of Writable)) As Boolean?
			Return Not filter.removeExample(v1) 'Spark: return true to keep example (Filter: return true to remove)
		End Function
	End Class

End Namespace