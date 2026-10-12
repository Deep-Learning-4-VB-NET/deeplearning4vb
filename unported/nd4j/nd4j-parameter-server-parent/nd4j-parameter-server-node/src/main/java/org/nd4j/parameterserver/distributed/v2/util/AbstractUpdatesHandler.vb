Imports System
Imports UpdatesHandler = org.nd4j.parameterserver.distributed.v2.transport.UpdatesHandler
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

Namespace org.nd4j.parameterserver.distributed.v2.util

	Public MustInherit Class AbstractUpdatesHandler
		Implements UpdatesHandler

		Public MustOverride ReadOnly Property ParametersArray As org.nd4j.linalg.api.ndarray.INDArray Implements UpdatesHandler.getParametersArray

		Public Overrides Sub onSubscribe(ByVal subscription As Subscription)

		End Sub

		Public Overrides Sub onError(ByVal throwable As Exception)

		End Sub

		Public Overrides Sub onComplete()

		End Sub
	End Class

End Namespace