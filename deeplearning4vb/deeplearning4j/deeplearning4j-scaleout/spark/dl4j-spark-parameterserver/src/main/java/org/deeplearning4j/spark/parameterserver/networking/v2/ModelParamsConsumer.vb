Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.function
Imports org.nd4j.common.primitives
Imports Subscriber = org.reactivestreams.Subscriber
Imports Subscription = org.reactivestreams.Subscription

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

Namespace org.deeplearning4j.spark.parameterserver.networking.v2

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ModelParamsConsumer implements org.reactivestreams.Subscriber<org.nd4j.linalg.api.ndarray.INDArray>, org.nd4j.common.function.Supplier<org.nd4j.linalg.api.ndarray.INDArray>
	Public Class ModelParamsConsumer
		Implements Subscriber(Of INDArray), Supplier(Of INDArray)

		<NonSerialized>
		Protected Friend ReadOnly params As New Atomic(Of INDArray)()

		Public Overrides Sub onSubscribe(ByVal subscription As Subscription)
			' no-op
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public synchronized void onNext(@NonNull INDArray array)
		Public Overrides Sub onNext(ByVal array As INDArray)
			SyncLock Me
				log.info("Storing params for future use...")
				If array IsNot Nothing Then
					params.set(array)
				End If
			End SyncLock
		End Sub

		Public Overrides Sub onError(ByVal throwable As Exception)
			Throw New Exception(throwable)
		End Sub

		Public Overrides Sub onComplete()
			' no-op
		End Sub

		Public Overridable Function get() As INDArray
			Return params.get()
		End Function
	End Class

End Namespace