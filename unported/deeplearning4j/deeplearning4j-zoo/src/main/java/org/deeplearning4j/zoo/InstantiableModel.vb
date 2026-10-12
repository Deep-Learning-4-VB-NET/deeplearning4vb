Imports System
Imports Model = org.deeplearning4j.nn.api.Model

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

Namespace org.deeplearning4j.zoo

	Public Interface InstantiableModel

		WriteOnly Property InputShape As Integer()()

		 Function init(Of M As Model)() As M

		''' @deprecated No longer used, will be removed in a future release 
		<Obsolete("No longer used, will be removed in a future release")>
		Function metaData() As ModelMetaData

		Function modelType() As Type

		Function pretrainedUrl(ByVal pretrainedType As PretrainedType) As String

		Function pretrainedChecksum(ByVal pretrainedType As PretrainedType) As Long

		Function modelName() As String
	End Interface

End Namespace