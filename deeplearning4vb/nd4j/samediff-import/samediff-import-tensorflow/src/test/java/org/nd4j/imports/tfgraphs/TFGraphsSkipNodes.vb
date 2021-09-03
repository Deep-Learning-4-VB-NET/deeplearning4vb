Imports System.Collections.Generic

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

Namespace org.nd4j.imports.tfgraphs


	Public Class TFGraphsSkipNodes

		Private Shared ReadOnly SKIP_NODE_MAP As IDictionary(Of String, IList(Of String)) = Collections.unmodifiableMap(New HashMapAnonymousInnerClass())

		Private Class HashMapAnonymousInnerClass
			Inherits Dictionary(Of String, IList(Of String))

			Public Sub New()

				Me.put("deep_mnist", New List(Of )(java.util.Arrays.asList("dropout/dropout/random_uniform/RandomUniform", "dropout/dropout/random_uniform/mul", "dropout/dropout/random_uniform", "dropout/dropout/add")))
			End Sub

		End Class

		Public Shared Function skipNode(ByVal modelName As String, ByVal varName As String) As Boolean

			If Not SKIP_NODE_MAP.Keys.Contains(modelName) Then
				Return False
			Else
				For Each some_node As String In SKIP_NODE_MAP(modelName)
					If some_node.Equals(varName) Then
						Return True
					End If
				Next some_node
				Return False
			End If

		End Function
	End Class

End Namespace