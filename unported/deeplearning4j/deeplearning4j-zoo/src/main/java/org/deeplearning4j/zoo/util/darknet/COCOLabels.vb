Imports System
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports BaseLabels = org.deeplearning4j.zoo.util.BaseLabels

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

Namespace org.deeplearning4j.zoo.util.darknet


	Public Class COCOLabels
		Inherits BaseLabels

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public COCOLabels() throws java.io.IOException
		Public Sub New()
			MyBase.New("coco.names")
		End Sub

		Protected Friend Overrides ReadOnly Property URL As URL
			Get
				Try
					Return DL4JResources.getURL("resources/darknet/coco.names")
				Catch e As MalformedURLException
					Throw New Exception(e)
				End Try
			End Get
		End Property

		Protected Friend Overrides Function resourceName() As String
			Return "darknet"
		End Function

		Protected Friend Overrides Function resourceMD5() As String
			Return "4caf6834300c8b2ff19964b36e54d637"
		End Function
	End Class

End Namespace