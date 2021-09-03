Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.io.labels


	Public Interface PathLabelGenerator

		Function getLabelForPath(ByVal path As String) As Writable

		Function getLabelForPath(ByVal uri As URI) As Writable

		''' <summary>
		''' If true: infer the set of possible label classes, and convert these to integer indexes. If when true, the
		''' returned Writables should be text writables.<br>
		''' <br>
		''' For regression use cases (or PathLabelGenerator classification instances that do their own label -> integer
		''' assignment), this should return false.
		''' </summary>
		''' <returns> whether label classes should be inferred </returns>
		Function inferLabelClasses() As Boolean

	End Interface

End Namespace