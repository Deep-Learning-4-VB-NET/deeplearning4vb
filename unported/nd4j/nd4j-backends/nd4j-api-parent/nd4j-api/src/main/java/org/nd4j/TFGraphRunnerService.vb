Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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


Namespace org.nd4j



	Public Interface TFGraphRunnerService
		Function init(ByVal inputNames As IList(Of String), ByVal outputNames As IList(Of String), ByVal graphBytes() As SByte, ByVal constants As IDictionary(Of String, INDArray), ByVal inputDataTypes As IDictionary(Of String, String)) As TFGraphRunnerService

		Function run(ByVal inputs As IDictionary(Of String, INDArray)) As IDictionary(Of String, INDArray)
	End Interface

End Namespace