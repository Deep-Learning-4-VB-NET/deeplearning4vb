Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ConvolutionLayer = org.deeplearning4j.nn.layers.convolution.ConvolutionLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingLayer
Imports BatchNormalization = org.deeplearning4j.nn.layers.normalization.BatchNormalization
Imports LocalResponseNormalization = org.deeplearning4j.nn.layers.normalization.LocalResponseNormalization
Imports LSTM = org.deeplearning4j.nn.layers.recurrent.LSTM
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.cuda


	''' <summary>
	''' Test utility methods specific to CuDNN
	''' 
	''' @author Alex Black
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class CuDNNTestUtils
	Public Class CuDNNTestUtils

		Private Sub New()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void removeHelpers(org.deeplearning4j.nn.api.Layer[] layers) throws Exception
		Public Shared Sub removeHelpers(ByVal layers() As Layer)
			For Each l As Layer In layers

				If TypeOf l Is ConvolutionLayer Then
					Dim f1 As System.Reflection.FieldInfo = GetType(ConvolutionLayer).getDeclaredField("helper")
					f1.setAccessible(True)
					f1.set(l, Nothing)
				ElseIf TypeOf l Is SubsamplingLayer Then
					Dim f2 As System.Reflection.FieldInfo = GetType(SubsamplingLayer).getDeclaredField("helper")
					f2.setAccessible(True)
					f2.set(l, Nothing)
				ElseIf TypeOf l Is BatchNormalization Then
					Dim f3 As System.Reflection.FieldInfo = GetType(BatchNormalization).getDeclaredField("helper")
					f3.setAccessible(True)
					f3.set(l, Nothing)
				ElseIf TypeOf l Is LSTM Then
					Dim f4 As System.Reflection.FieldInfo = GetType(LSTM).getDeclaredField("helper")
					f4.setAccessible(True)
					f4.set(l, Nothing)
				ElseIf TypeOf l Is LocalResponseNormalization Then
					Dim f5 As System.Reflection.FieldInfo = GetType(LocalResponseNormalization).getDeclaredField("helper")
					f5.setAccessible(True)
					f5.set(l, Nothing)
				End If


				If l.Helper IsNot Nothing Then
					Throw New System.InvalidOperationException("Did not remove helper for layer: " & l.GetType().Name)
				End If
			Next l
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void assertHelpersPresent(org.deeplearning4j.nn.api.Layer[] layers) throws Exception
		Public Shared Sub assertHelpersPresent(ByVal layers() As Layer)
			For Each l As Layer In layers
				'Don't use instanceof here - there are sub conv subclasses
				If l.GetType() = GetType(ConvolutionLayer) OrElse TypeOf l Is SubsamplingLayer OrElse TypeOf l Is BatchNormalization OrElse TypeOf l Is LSTM Then
					Preconditions.checkNotNull(l.Helper, l.conf().getLayer().getLayerName())
				End If
			Next l
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void assertHelpersAbsent(org.deeplearning4j.nn.api.Layer[] layers) throws Exception
		Public Shared Sub assertHelpersAbsent(ByVal layers() As Layer)
			For Each l As Layer In layers
				Preconditions.checkState(l.Helper Is Nothing, l.conf().getLayer().getLayerName())
			Next l
		End Sub

	End Class

End Namespace