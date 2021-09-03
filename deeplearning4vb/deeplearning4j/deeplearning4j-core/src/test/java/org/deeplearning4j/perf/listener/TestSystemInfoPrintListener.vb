Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports SystemInfoFilePrintListener = org.deeplearning4j.core.listener.SystemInfoFilePrintListener
Imports SystemInfoPrintListener = org.deeplearning4j.core.listener.SystemInfoPrintListener
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.perf.listener


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("AB 2019/05/24 - Failing on CI - ""Could not initialize class oshi.jna.platform.linux.Libc"" - Issue #7657") public class TestSystemInfoPrintListener extends org.deeplearning4j.BaseDL4JTest
	Public Class TestSystemInfoPrintListener
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testListener(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testListener(ByVal testDir As Path)
			Dim systemInfoPrintListener As SystemInfoPrintListener = SystemInfoPrintListener.builder().printOnEpochStart(True).printOnEpochEnd(True).build()

			Dim tmpFile As File = Files.createTempFile(testDir,"tmpfile-log","txt").toFile()
			assertEquals(0, tmpFile.length())

			Dim systemInfoFilePrintListener As SystemInfoFilePrintListener = SystemInfoFilePrintListener.builder().printOnEpochStart(True).printOnEpochEnd(True).printFileTarget(tmpFile).build()
			tmpFile.deleteOnExit()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()
			net.setListeners(systemInfoFilePrintListener)

			Dim iter As DataSetIterator = New IrisDataSetIterator(10, 150)

			net.fit(iter, 3)

	'        System.out.println(FileUtils.readFileToString(tmpFile));
		End Sub

	End Class

End Namespace