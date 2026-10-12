Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Getter = lombok.Getter

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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @AllArgsConstructor @Deprecated public class ModelMetaData
	<Obsolete>
	Public Class ModelMetaData
		Private inputShape()() As Integer
		Private numOutputs As Integer
		Private zooType As ZooType

		''' <summary>
		''' If number of inputs are greater than 1, this states that the
		''' implementation should use MultiDataSet.
		''' @return
		''' </summary>
		Public Overridable Function useMDS() As Boolean
			Return If(inputShape.Length > 1, True, False)
		End Function
	End Class

End Namespace