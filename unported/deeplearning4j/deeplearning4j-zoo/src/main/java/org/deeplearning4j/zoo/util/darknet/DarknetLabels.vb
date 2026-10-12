Imports System
Imports System.Collections.Generic
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


	Public Class DarknetLabels
		Inherits BaseLabels

		Private shortNames As Boolean
		Private numClasses As Integer

		''' <summary>
		''' Calls {@code this(true)}.
		''' Defaults to 1000 clasess
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public DarknetLabels() throws java.io.IOException
		Public Sub New()
			Me.New(True)
		End Sub

		''' <param name="numClasses"> Number of classes (usually 1000 or 9000, depending on the model) </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public DarknetLabels(int numClasses) throws java.io.IOException
		Public Sub New(ByVal numClasses As Integer)
			Me.New(True, numClasses)
		End Sub

		Protected Friend Overrides ReadOnly Property URL As URL
			Get
				Try
					If shortNames Then
						Return DL4JResources.getURL("resources/darknet/imagenet.shortnames.list")
					Else
						Return DL4JResources.getURL("resources/darknet/imagenet.labels.list")
					End If
				Catch e As MalformedURLException
					Throw New Exception(e)
				End Try
			End Get
		End Property

		''' <param name="shortnames"> if true, uses "imagenet.shortnames.list", otherwise "imagenet.labels.list". </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public DarknetLabels(boolean shortnames) throws java.io.IOException
		Public Sub New(ByVal shortnames As Boolean)
			Me.New(shortnames, 1000)
		End Sub

		''' <param name="shortnames"> if true, uses "imagenet.shortnames.list", otherwise "imagenet.labels.list". </param>
		''' <param name="numClasses"> Number of classes (usually 1000 or 9000, depending on the model) </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public DarknetLabels(boolean shortnames, int numClasses) throws java.io.IOException
		Public Sub New(ByVal shortnames As Boolean, ByVal numClasses As Integer)
			Me.shortNames = shortnames
			Me.numClasses = numClasses
			Dim labels As IList(Of String) = getLabels(If(shortnames, "imagenet.shortnames.list", "imagenet.labels.list"))
			Me.labels = New List(Of String)()
			For i As Integer = 0 To numClasses - 1
				Me.labels.Add(labels(i))
			Next i
		End Sub

		Protected Friend Overrides Function resourceName() As String
			Return "darknet"
		End Function

		Protected Friend Overrides Function resourceMD5() As String
			If shortNames Then
				Return "23d2a102a2de03d1b169c748b7141a20"
			Else
				Return "23ab429a707492324fef60a933551941"
			End If
		End Function
	End Class

End Namespace