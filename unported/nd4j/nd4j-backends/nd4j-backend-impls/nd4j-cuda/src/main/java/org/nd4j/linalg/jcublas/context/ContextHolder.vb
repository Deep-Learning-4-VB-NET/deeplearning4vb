Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.nd4j.linalg.jcublas.context



	''' <summary>
	''' A multithreaded version derived
	''' from the cuda launcher util
	''' by the authors of jcuda.
	''' <para>
	''' This class handles managing cuda contexts
	''' across multiple devices and threads.
	''' 
	''' @author Adam Gibson
	''' </para>
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class ContextHolder
	Public Class ContextHolder

		Private threadNameToDeviceNumber As IDictionary(Of String, Integer) = New ConcurrentDictionary(Of String, Integer)()
		Private threads As IDictionary(Of String, Integer) = New ConcurrentDictionary(Of String, Integer)()
		Private bannedDevices As IList(Of Integer)
'JAVA TO VB CONVERTER NOTE: The field numDevices was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private numDevices_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared INSTANCE_Conflict As ContextHolder
		Public Const DEVICES_TO_BAN As String = "org.nd4j.linalg.jcuda.jcublas.ban_devices"
		Private Shared deviceSetup As New AtomicBoolean(False)
		Private confCalled As Boolean = False
		Private Shared log As Logger = LoggerFactory.getLogger(GetType(ContextHolder))
		Private shutdown As New AtomicBoolean(False)

		' holder for memory strategies override

		''' <summary>
		''' Singleton pattern
		''' </summary>
		''' <returns> the instance for the context holder. </returns>
		Public Shared ReadOnly Property Instance As ContextHolder
			Get
				SyncLock GetType(ContextHolder)
            
					If INSTANCE_Conflict Is Nothing Then
						Dim props As New Properties()
						Try
							props.load((New ClassPathResource("/cudafunctions.properties", GetType(ContextHolder).getClassLoader())).InputStream)
						Catch e As IOException
							Throw New Exception(e)
						End Try
            
						INSTANCE_Conflict = New ContextHolder()
						INSTANCE_Conflict.configure()
            
            
						'set the properties to be accessible globally
						For Each pair As String In props.stringPropertyNames()
							System.getProperties().put(pair, props.getProperty(pair))
						Next pair
            
            
					End If
            
            
					Return INSTANCE_Conflict
				End SyncLock
			End Get
		End Property


		Public Overridable ReadOnly Property Threads As IDictionary(Of String, Integer)
			Get
				Return threads
			End Get
		End Property


		''' <summary>
		''' Get the number of devices
		''' </summary>
		''' <returns> the number of devices </returns>
		Public Overridable Function deviceNum() As Integer
			Return numDevices_Conflict
		End Function


		''' <summary>
		''' Configure the given information
		''' based on the device
		''' </summary>
		Public Overridable Sub configure()
			If confCalled Then
				Return
			End If

	'
	'        setContext();
	'
	'
	'
	'        // Check if the device supports mapped host memory
	'        cudaDeviceProp deviceProperties = new cudaDeviceProp();
	'        JCuda.cudaGetDeviceProperties(deviceProperties, 0);
	'        if (deviceProperties.canMapHostMemory == 0) {
	'            System.err.println("This device can not map host memory");
	'            System.err.println(deviceProperties.toFormattedString());
	'            return;
	'        }
	'

	'        
	'        // if we'll need stack initialization, here's the code
	'        int numberOfCores = CudaArgs.convertMPtoCores(deviceProperties.major, deviceProperties.minor, deviceProperties.multiProcessorCount) * deviceProperties.multiProcessorCount;
	'        int maxThreadsPerCore = deviceProperties.maxThreadsPerMultiProcessor / CudaArgs.convertMPtoCores(deviceProperties.major, deviceProperties.minor, deviceProperties.multiProcessorCount);
	'
	'
	'        long stackSize = Math.min(512*1024, deviceProperties.totalGlobalMem / numberOfCores  / (maxThreadsPerCore + 8) );
	'
	'        JCuda.cudaDeviceSetLimit(0,stackSize);
	'
	'        


			'force certain ops to have a certain number of threads
	'        
	'        Properties threadProps = new Properties();
	'        try {
	'            InputStream is = ContextHolder.class.getResourceAsStream("/function_threads.properties");
	'            threadProps.load(is);
	'        } catch (IOException e) {
	'            e.printStackTrace();
	'        }
	'
	'        for(String prop : threadProps.stringPropertyNames()) {
	'            threads.put(prop,Integer.parseInt(threadProps.getProperty(prop)));
	'        }
	'        

			Try
	'            
	'            GenericObjectPoolConfig config = new GenericObjectPoolConfig();
	'            config.setJmxEnabled(true);
	'            config.setBlockWhenExhausted(false);
	'            config.setMaxIdle(Runtime.getRuntime().availableProcessors());
	'            config.setMaxTotal(Runtime.getRuntime().availableProcessors());
	'            config.setMinIdle(Runtime.getRuntime().availableProcessors());
	'            config.setJmxNameBase("handles");
	'            handlePool = new CublasHandlePool(new CublasHandlePooledItemFactory(),config);
	'            GenericObjectPoolConfig confClone = config.clone();
	'            confClone.setMaxTotal(Runtime.getRuntime().availableProcessors() * 10);
	'            confClone.setMaxIdle(Runtime.getRuntime().availableProcessors() * 10);
	'            confClone.setMinIdle(Runtime.getRuntime().availableProcessors() * 10);
	'            GenericObjectPoolConfig streamConf = confClone.clone();
	'            streamConf.setJmxNameBase("streams");
	'            streamPool = new StreamPool(new StreamItemFactory(),streamConf);
	'            GenericObjectPoolConfig oldStreamConf = streamConf.clone();
	'            oldStreamConf.setJmxNameBase("oldstream");
	'            oldStreamPool = new OldStreamPool(new OldStreamItemFactory(),oldStreamConf);
	'            setContext();
	'            //seed with multiple streams to encourage parallelism
	'            for(int i = 0; i < Runtime.getRuntime().availableProcessors(); i++) {
	'                streamPool.addObject();
	'                oldStreamPool.addObject();
	'            }
	'

			Catch e As Exception
				log.warn("Unable to initialize cuda", e)
			End Try

	'    
	'        for(int i = 0; i < numDevices; i++) {
	'            ClassPathResource confFile = new ClassPathResource("devices/" + i, ContextHolder.class.getClassLoader());
	'            if(confFile.exists()) {
	'                Properties props2 = new Properties();
	'                try {
	'                    props2.load(confFile.getInputStream());
	'                    confs.put(i,new DeviceConfiguration(i,props2));
	'                } catch (IOException e) {
	'                    throw new RuntimeException(e);
	'                }
	'
	'            }
	'            else
	'                confs.put(i,new DeviceConfiguration(i));
	'
	'        }
	'        

			confCalled = True
		End Sub

		Public Overridable WriteOnly Property NumDevices As Integer
			Set(ByVal numDevices As Integer)
				Me.numDevices_Conflict = numDevices
			End Set
		End Property

		''' <summary>
		''' Get the device number for a particular host thread
		''' </summary>
		''' <returns> the device for the given host thread </returns>
		Public Overridable ReadOnly Property DeviceForThread As Integer
			Get
		'        
		'        if(numDevices > 1) {
		'            Integer device =  threadNameToDeviceNumber.get(Thread.currentThread().getName());
		'            if(device == null) {
		'                org.nd4j.linalg.api.rng.Random random = Nd4j.getRandom();
		'                if(random == null)
		'                    throw new IllegalStateException("Unable to load random class");
		'                device = Nd4j.getRandom().nextInt(numDevices);
		'                //reroute banned devices
		'                while(bannedDevices != null && bannedDevices.contains(device))
		'                    device = Nd4j.getRandom().nextInt(numDevices);
		'                threadNameToDeviceNumber.put(Thread.currentThread().getName(),device);
		'                return device;
		'            }
		'        }
		'
				Return 0
			End Get
		End Property
	End Class

End Namespace