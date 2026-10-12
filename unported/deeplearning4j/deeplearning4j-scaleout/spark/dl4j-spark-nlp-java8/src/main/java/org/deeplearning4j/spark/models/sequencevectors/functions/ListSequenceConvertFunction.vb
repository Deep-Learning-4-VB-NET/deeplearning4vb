Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement

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

Namespace org.deeplearning4j.spark.models.sequencevectors.functions


	Public Class ListSequenceConvertFunction(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements [Function](Of IList(Of T), Sequence(Of T))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.models.sequencevectors.sequence.Sequence<T> call(java.util.List<T> ts) throws Exception
		Public Overrides Function [call](ByVal ts As IList(Of T)) As Sequence(Of T)
			Dim sequence As New Sequence(Of T)(ts)
			Return sequence
		End Function
	End Class

End Namespace