Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports PortableDataStreamCallback = org.deeplearning4j.spark.parameterserver.callbacks.PortableDataStreamCallback
Imports DataSet = org.nd4j.linalg.dataset.DataSet

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

Namespace org.deeplearning4j.spark.parameterserver.iterators


	Public Class PdsIterator
		Implements IEnumerator(Of DataSet)

		Protected Friend ReadOnly iterator As IEnumerator(Of PortableDataStream)
		Protected Friend ReadOnly callback As PortableDataStreamCallback

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public PdsIterator(@NonNull Iterator<org.apache.spark.input.PortableDataStream> pds, @NonNull PortableDataStreamCallback callback)
		Public Sub New(ByVal pds As IEnumerator(Of PortableDataStream), ByVal callback As PortableDataStreamCallback)
			Me.iterator = pds
			Me.callback = callback
		End Sub

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iterator.hasNext()
		End Function

		Public Overrides Function [next]() As DataSet
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return callback.compute(iterator.next())
		End Function

		Public Overrides Sub remove()
			' no-op
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? super org.nd4j.linalg.dataset.DataSet> action)
		Public Overrides Sub forEachRemaining(Of T1)(ByVal action As System.Action(Of T1))
			Throw New System.NotSupportedException()
		End Sub
	End Class

End Namespace