Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.spark.text.functions


	''' <summary>
	''' @author jeffreytang
	''' </summary>
	Public Class GetSentenceCountFunction
		Implements [Function](Of Pair(Of IList(Of String), AtomicLong), AtomicLong)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.concurrent.atomic.AtomicLong call(org.nd4j.common.primitives.Pair<java.util.List<String>, java.util.concurrent.atomic.AtomicLong> pair) throws Exception
		Public Overrides Function [call](ByVal pair As Pair(Of IList(Of String), AtomicLong)) As AtomicLong
			Return pair.Second
		End Function
	End Class

End Namespace