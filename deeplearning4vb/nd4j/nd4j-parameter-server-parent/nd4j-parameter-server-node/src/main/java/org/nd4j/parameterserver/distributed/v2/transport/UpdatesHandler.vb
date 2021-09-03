Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Subscriber = org.reactivestreams.Subscriber

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

Namespace org.nd4j.parameterserver.distributed.v2.transport

	Public Interface UpdatesHandler
		Inherits Subscriber(Of INDArray)

		''' <summary>
		''' This method returns parameters array maintained by this handler
		''' @return
		''' </summary>
		ReadOnly Property ParametersArray As INDArray
	End Interface

End Namespace