Imports System.Collections.Generic
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports SequenceSplit = org.datavec.api.transform.sequence.SequenceSplit
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

Namespace org.datavec.spark.transform.transform


	Public Class SequenceSplitFunction
		Implements FlatMapFunction(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable)))

		Private ReadOnly split As SequenceSplit

		Public Sub New(ByVal split As SequenceSplit)
			Me.split = split
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<java.util.List<java.util.List<org.datavec.api.writable.Writable>>> call(java.util.List<java.util.List<org.datavec.api.writable.Writable>> collections) throws Exception
		Public Overrides Function [call](ByVal collections As IList(Of IList(Of Writable))) As IEnumerator(Of IList(Of IList(Of Writable)))
			Return split.split(collections).GetEnumerator()
		End Function

	End Class

End Namespace