Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Test = org.junit.Test
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports Tag = org.junit.jupiter.api.Tag
Imports Resources = org.nd4j.common.resources.Resources
Imports ResourceFile = org.nd4j.common.resources.strumpf.ResourceFile
Imports StrumpfResolver = org.nd4j.common.resources.strumpf.StrumpfResolver
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.Assert.assertArrayEquals

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
Namespace org.deeplearning4j.nn.modelimport.keras.layers.convolution


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Separable Convolution 3D Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag public class KerasDeconvolution3DTest extends org.deeplearning4j.BaseDL4JTest
	Public Class KerasDeconvolution3DTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDeconv3D() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDeconv3D()
			Dim f As File = Resources.asFile("/modelimport/keras/weights/conv3d_transpose.h5")
			Dim multiLayerNetwork As MultiLayerNetwork = KerasModelImport.importKerasSequentialModelAndWeights(f.getAbsolutePath(), True)
			Console.WriteLine(multiLayerNetwork.summary())
			Dim output As INDArray = multiLayerNetwork.output(Nd4j.ones(1, 100))
			assertArrayEquals(New Long(){1, 30, 30, 30, 64},output.shape())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDeconv3DNCHW() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDeconv3DNCHW()
			Dim f As File = Resources.asFile("/modelimport/keras/weights/conv3d_transpose_nchw.h5")
			Dim multiLayerNetwork As MultiLayerNetwork = KerasModelImport.importKerasSequentialModelAndWeights(f.getAbsolutePath(), True)
			Console.WriteLine(multiLayerNetwork.summary())
			Dim output As INDArray = multiLayerNetwork.output(Nd4j.ones(1, 100))
			assertArrayEquals(New Long(){1, 64, 33, 33, 1539},output.shape())

		End Sub

	End Class

End Namespace